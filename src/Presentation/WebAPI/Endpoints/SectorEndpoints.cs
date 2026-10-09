// <copyright file="SectorEndpoints.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Endpoints;

using Microsoft.AspNetCore.Mvc;
using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Application.Sectors.Dtos;
using Tfs.Portfolio.Application.Sectors.Queries;
using Tfs.Portfolio.Application.Sectors.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Sectors.Repositories;
using AutoMapper;
using Tfs.Portfolio.Api.Filters;

/// <summary>
/// Sector endpoints.
/// </summary>
internal static class SectorEndpoints
{
    /// <summary>
    /// Maps the sector endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapSectorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/sectors").WithTags("Sectors");

        group.MapGet("/", async ([FromServices] GetSectorsQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetSectorsQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetSectors")
        .Produces<IReadOnlyList<SectorListItemDto>>();

        group.MapGet("/active", async ([FromServices] GetActiveSectorsQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetActiveSectorsQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetActiveSectors")
        .Produces<IReadOnlyList<SectorListItemDto>>();

        group.MapGet("/{id:guid}", async (Guid id, [FromServices] GetSectorByIdQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetSectorByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.Problem(
                title: "Not Found",
                detail: $"Sector with ID '{id}' was not found.",
                statusCode: 404,
                instance: $"/api/v1/sectors/{id}");
        })
        .WithName("GetSectorById")
        .Produces<SectorDto>()
        .Produces(404);

        group.MapPost("/", async (CreateSectorCommand command, [FromServices] CreateSectorCommandHandler handler, [FromServices] IMapper mapper, [FromServices] ISectorRepository sectorRepository, CancellationToken ct) =>
        {
            var sectorId = await handler.HandleAsync(command, ct);
            var sector = await sectorRepository.GetByIdAsync(sectorId, ct);
            var sectorDto = mapper.Map<SectorDto>(sector!);
            return Results.Created($"/api/v1/sectors/{sectorId}", sectorDto);
        })
        .WithName("CreateSector")
        .Accepts<CreateSectorCommand>("application/json")
        .Produces(201)
        .Produces<ProblemDetails>(400)
        .AddEndpointFilter<ValidationFilter<CreateSectorCommand>>();

        group.MapPut("/{id:guid}", async (Guid id, UpdateSectorCommand command, [FromServices] UpdateSectorCommandHandler handler, CancellationToken ct) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(new ProblemDetails { Title = "ID mismatch", Detail = "Route ID does not match command ID." });
            }
            await handler.HandleAsync(command, ct);
            return Results.NoContent();
        })
        .WithName("UpdateSector")
        .Accepts<UpdateSectorCommand>("application/json")
        .Produces(204)
        .Produces<ProblemDetails>(400)
        .Produces(404)
        .AddEndpointFilter<ValidationFilter<UpdateSectorCommand>>();

        group.MapDelete("/{id:guid}", async (Guid id, [FromServices] DeleteSectorCommandHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new DeleteSectorCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeleteSector")
        .Produces(204)
        .Produces(404);

        return endpoints;
    }
}