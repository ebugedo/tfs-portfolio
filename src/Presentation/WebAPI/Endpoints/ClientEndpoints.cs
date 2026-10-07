// <copyright file="ClientEndpoints.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Endpoints;

using Microsoft.AspNetCore.Mvc;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Application.Clients.Queries;
using Tfs.Portfolio.Application.Clients.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Clients.Repositories;
using AutoMapper;
using Tfs.Portfolio.Api.Filters;

/// <summary>
/// Client endpoints.
/// </summary>
internal static class ClientEndpoints
{
    /// <summary>
    /// Maps the client endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/clients").WithTags("Clients");

        group.MapGet("/", async ([FromServices] GetClientsQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetClientsQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetClients")
        .Produces<IReadOnlyList<ClientListItemDto>>();

        group.MapGet("/active", async ([FromServices] GetActiveClientsQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetActiveClientsQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetActiveClients")
        .Produces<IReadOnlyList<ClientListItemDto>>();

        group.MapGet("/{id:guid}", async (Guid id, [FromServices] GetClientByIdQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetClientByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.Problem(
                title: "Not Found",
                detail: $"Client with ID '{id}' was not found.",
                statusCode: 404,
                instance: $"/api/v1/clients/{id}");
        })
        .WithName("GetClientById")
        .Produces<ClientDto>()
        .Produces(404);

        group.MapPost("/", async (CreateClientCommand command, [FromServices] CreateClientCommandHandler handler, [FromServices] IMapper mapper, [FromServices] IClientRepository clientRepository, CancellationToken ct) =>
        {
            var clientId = await handler.HandleAsync(command, ct);
            var client = await clientRepository.GetByIdAsync(clientId, ct);
            var clientDto = mapper.Map<ClientDto>(client!);
            return Results.Created($"/api/v1/clients/{clientId}", clientDto);
        })
        .WithName("CreateClient")
        .Accepts<CreateClientCommand>("application/json")
        .Produces(201)
        .Produces<ProblemDetails>(400)
        .AddEndpointFilter<ValidationFilter<CreateClientCommand>>();

        group.MapPut("/{id:guid}", async (Guid id, UpdateClientCommand command, [FromServices] UpdateClientCommandHandler handler, CancellationToken ct) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(new ProblemDetails { Title = "ID mismatch", Detail = "Route ID does not match command ID." });
            }
            await handler.HandleAsync(command, ct);
            return Results.NoContent();
        })
        .WithName("UpdateClient")
        .Accepts<UpdateClientCommand>("application/json")
        .Produces(204)
        .Produces<ProblemDetails>(400)
        .Produces(404)
        .AddEndpointFilter<ValidationFilter<UpdateClientCommand>>();

        group.MapPut("/{id:guid}/deactivate", async (Guid id, [FromServices] DeactivateClientCommandHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new DeactivateClientCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeactivateClient")
        .Produces(204)
        .Produces(404);

        group.MapDelete("/{id:guid}", async (Guid id, [FromServices] DeleteClientCommandHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new DeleteClientCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeleteClient")
        .Produces(204)
        .Produces(404);

        return endpoints;
    }
}