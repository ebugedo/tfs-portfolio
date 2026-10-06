// <copyright file="Program.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Tfs.Portfolio.Api.Filters;
using Tfs.Portfolio.Api.Middleware;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Common.Modules;
using Tfs.Portfolio.Application.Common.Services;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Projects.Handlers;
using Tfs.Portfolio.Application.Projects.Queries;
using Tfs.Portfolio.Infrastructure.Persistence;
using Tfs.Portfolio.Infrastructure.Persistence.Modules;

/// <summary>
/// Program entry point.
/// </summary>
public sealed class Program
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console(formatProvider: System.Globalization.CultureInfo.InvariantCulture)
            .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day, formatProvider: System.Globalization.CultureInfo.InvariantCulture)
            .WriteTo.Seq(
                serverUrl: context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341",
                apiKey: context.Configuration["Seq:ApiKey"]));

        // Use Autofac as the DI container
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        // Add services
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Tfs.Portfolio API",
                    Version = "v1",
                    Description = "Portfolio Management API"
                };
                return Task.CompletedTask;
            });
        });

        // Add health checks
        builder.Services.AddHealthChecks()
            .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

        // Add ApplicationDbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseNpgsql(connectionString);
            }
        });

        // Add problem details for error handling
        builder.Services.AddProblemDetails();

        // Configure Autofac
        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder.RegisterModule(new ApplicationModule());
            containerBuilder.RegisterModule(new PersistenceModule(builder.Configuration));

            // Register application services
            containerBuilder.RegisterType<CurrentUserService>()
                .As<ICurrentUserService>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<DateTimeProvider>()
                .As<IDateTimeProvider>()
                .InstancePerLifetimeScope();
        });

        var app = builder.Build();

        // Configure middleware pipeline
        if (app.Environment.IsDevelopment())
        {
            // Apply pending migrations automatically in Development
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "Tfs.Portfolio API";
                options.Theme = ScalarTheme.DeepSpace;
            });
        }

        app.UseExceptionHandler("/error");
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseRouting();

        // Comentado hasta implementar autenticación
        // app.UseAuthorization();

        // Map endpoints
        app.MapControllers();
        app.MapHealthChecks("/health/live");
        app.MapHealthChecks("/health/ready");

        // Test endpoint simple
        app.MapGet("/test", () => Results.Ok(new { message = "API funcionando - Hot reload works!", timestamp = DateTime.UtcNow }));

        // Map versioned endpoints
        var v1 = app.MapGroup("/api/v1").WithTags("v1");
        v1.MapProjectEndpoints();

        app.Run();
    }
}

// Extension methods for endpoint mapping
internal static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/projects").WithTags("Projects");

        group.MapGet("/", async ([FromServices] GetProjectsQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetProjectsQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetProjects")
        .Produces<IReadOnlyList<ProjectListItemDto>>();

        group.MapGet("/{id:guid}", async (Guid id, [FromServices] GetProjectByIdQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetProjectByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetProjectById")
        .Produces<ProjectDto>()
        .Produces(404);

        group.MapPost("/", async (CreateProjectCommand command, [FromServices] CreateProjectCommandHandler handler, CancellationToken ct) =>
        {
            var projectId = await handler.HandleAsync(command, ct);
            return Results.Created($"/api/v1/projects/{projectId}", new { id = projectId });
        })
        .WithName("CreateProject")
        .Accepts<CreateProjectCommand>("application/json")
        .Produces(201)
        .Produces<ProblemDetails>(400)
        .AddEndpointFilter<ValidationFilter<CreateProjectCommand>>();

        group.MapPut("/{id:guid}", async (Guid id, UpdateProjectCommand command, [FromServices] UpdateProjectCommandHandler handler, CancellationToken ct) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(new ProblemDetails { Title = "ID mismatch", Detail = "Route ID does not match command ID." });
            }
            await handler.HandleAsync(command, ct);
            return Results.NoContent();
        })
        .WithName("UpdateProject")
        .Accepts<UpdateProjectCommand>("application/json")
        .Produces(204)
        .Produces<ProblemDetails>(400)
        .Produces(404)
        .AddEndpointFilter<ValidationFilter<UpdateProjectCommand>>();

        group.MapDelete("/{id:guid}", async (Guid id, [FromServices] DeleteProjectCommandHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new DeleteProjectCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeleteProject")
        .Produces(204)
        .Produces(404);

        return endpoints;
    }
}