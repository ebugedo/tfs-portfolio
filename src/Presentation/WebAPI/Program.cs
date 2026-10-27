// <copyright file="Program.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Tfs.Portfolio.Api.Filters;
using Tfs.Portfolio.Api.Middleware;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Common.Modules;
using Tfs.Portfolio.Application.Common.Services;

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
                options.SerializerSettings.ContractResolver = new Tfs.Portfolio.Api.Json.TfsWebUrlContractResolver();
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

// Add FluentValidation - register validators from Application assembly in ASP.NET Core DI
        // This is needed for [FromServices] injection in controllers when using Autofac
        builder.Services.AddValidatorsFromAssemblyContaining<Tfs.Portfolio.Application.Clients.Validators.CreateClientCommandValidator>();

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

                // Add X-API-Key security scheme
                if (document.Components == null)
                {
                    document.Components = new OpenApiComponents();
                }

                document.Components.SecuritySchemes["X-API-Key"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "X-API-Key",
                    In = ParameterLocation.Header,
                    Description = "API Key required for protected endpoints (POST, PUT, DELETE, PATCH)"
                };

                // Apply security requirement to write operations only
                var writeMethods = new HashSet<string> { "POST", "PUT", "DELETE", "PATCH" };
                var apiKeyRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "X-API-Key"
                            }
                        },
                        Array.Empty<string>()
                    }
                };

                foreach (var (path, pathItem) in document.Paths)
                {
                    foreach (var (httpMethod, operation) in pathItem.Operations)
                    {
                        if (writeMethods.Contains(httpMethod.ToString().ToUpperInvariant()))
                        {
                            operation.Security ??= new List<OpenApiSecurityRequirement>();
                            operation.Security.Add(apiKeyRequirement);
                        }
                    }
                }

                return Task.CompletedTask;
            });
        });

        // Add health checks
        builder.Services.AddHealthChecks()
            .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

        // Opt Npgsql into dynamic JSON serialization for JSONB columns
        // (required to read/write List<T> properties such as Project.Technologies).
        // Npgsql does not expose a Newtonsoft.Json switch for JSONB; dynamic JSON is the
        // supported opt-in and matches test/IntegrationTests/CustomWebApplicationFactory.cs.
#pragma warning disable CS0618 // Type or member is obsolete
        NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
#pragma warning restore CS0618

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

        // Validate API Key configuration in Production environments
        if (builder.Environment.IsProduction())
        {
            var apiKey = builder.Configuration["ApiKeySettings:MasterKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "ApiKeySettings:MasterKey is required in Production environments. " +
                    "Set the ApiKeySettings__MasterKey environment variable.");
            }
        }

        var app = builder.Build();

        // Apply pending migrations automatically on startup (all environments except Testing)
        if (!app.Environment.IsEnvironment("Testing"))
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }
        }

        // Configure middleware pipeline
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Tfs.Portfolio API";
            options.Theme = ScalarTheme.DeepSpace;
        });

        app.UseErrorHandling();
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseApiKey();

        // Comentado hasta implementar autenticación basada en tokens
        // app.UseAuthorization();

        // Map endpoints
        app.MapControllers();
        app.MapHealthChecks("/health/live");
        app.MapHealthChecks("/health/ready");

        // Test endpoint simple
        app.MapGet("/test", () => Results.Ok(new { message = "API funcionando - Hot reload works! Updated!", timestamp = DateTime.UtcNow }));

        app.Run();
    }
}

// Extension methods for endpoint mapping
internal static class EndpointExtensions
{
}
