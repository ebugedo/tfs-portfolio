// <copyright file="ServiceEndpoints.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Endpoints;

using Microsoft.AspNetCore.Mvc;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Application.Services.Queries;
using Tfs.Portfolio.Application.Services.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.Domain.Services.Repositories;
using AutoMapper;
using Tfs.Portfolio.Api.Filters;

/// <summary>
/// Service endpoints.
/// </summary>
internal static class ServiceEndpoints
{
    /// <summary>
    /// Maps the service endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/services").WithTags("Services");

        group.MapGet("/", async ([FromServices] GetServicesQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetServicesQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetServices")
        .Produces<IReadOnlyList<ServiceListItemDto>>();

        group.MapGet("/active", async ([FromServices] GetActiveServicesQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetActiveServicesQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetActiveServices")
        .Produces<IReadOnlyList<ServiceListItemDto>>();

        group.MapGet("/category/{category}", async (ServiceCategory category, [FromServices] GetServicesByCategoryQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetServicesByCategoryQuery(category), ct);
            return Results.Ok(result);
        })
        .WithName("GetServicesByCategory")
        .Produces<IReadOnlyList<ServiceListItemDto>>();

        group.MapGet("/{id:guid}", async (Guid id, [FromServices] GetServiceByIdQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetServiceByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.Problem(
                title: "Not Found",
                detail: $"Service with ID '{id}' was not found.",
                statusCode: 404,
                instance: $"/api/v1/services/{id}");
        })
        .WithName("GetServiceById")
        .Produces<ServiceDto>()
        .Produces(404);

        group.MapPost("/", async (CreateServiceCommand command, [FromServices] CreateServiceCommandHandler handler, [FromServices] IMapper mapper, [FromServices] IServiceRepository serviceRepository, CancellationToken ct) =>
        {
            var serviceId = await handler.HandleAsync(command, ct);
            var service = await serviceRepository.GetByIdAsync(serviceId, ct);
            var serviceDto = mapper.Map<ServiceDto>(service!);
            return Results.Created($"/api/v1/services/{serviceId}", serviceDto);
        })
        .WithName("CreateService")
        .Accepts<CreateServiceCommand>("application/json")
        .Produces(201)
        .Produces<ProblemDetails>(400)
        .AddEndpointFilter<ValidationFilter<CreateServiceCommand>>();

        group.MapPut("/{id:guid}", async (Guid id, UpdateServiceCommand command, [FromServices] UpdateServiceCommandHandler handler, CancellationToken ct) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(new ProblemDetails { Title = "ID mismatch", Detail = "Route ID does not match command ID." });
            }
            await handler.HandleAsync(command, ct);
            return Results.NoContent();
        })
        .WithName("UpdateService")
        .Accepts<UpdateServiceCommand>("application/json")
        .Produces(204)
        .Produces<ProblemDetails>(400)
        .Produces(404)
        .AddEndpointFilter<ValidationFilter<UpdateServiceCommand>>();

        group.MapDelete("/{id:guid}", async (Guid id, [FromServices] DeleteServiceCommandHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new DeleteServiceCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeleteService")
        .Produces(204)
        .Produces(404);

        return endpoints;
    }
}