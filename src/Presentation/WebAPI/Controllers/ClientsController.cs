// <copyright file="ClientsController.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Controllers;

using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Application.Clients.Queries;
using Tfs.Portfolio.Application.Clients.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Clients.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing clients.
/// </summary>
[ApiController]
[Route("api/v1/clients")]
[Tags("Clients")]
public sealed class ClientsController : ControllerBase
{
    /// <summary>
    /// Gets all clients.
    /// </summary>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of clients.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClientListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClients(
        [FromServices] GetClientsQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetClientsQuery(), CancellationToken.None);
        return Ok(result);
    }

    /// <summary>
    /// Gets all active clients.
    /// </summary>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of active clients.</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<ClientListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveClients(
        [FromServices] GetActiveClientsQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetActiveClientsQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a client by ID.
    /// </summary>
    /// <param name="id">The client ID.</param>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The client if found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientById(
        Guid id,
        [FromServices] GetClientByIdQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetClientByIdQuery(id), ct);
        return result is not null ? Ok(result) : NotFound(new ProblemDetails
        {
            Title = "Not Found",
            Detail = $"Client with ID '{id}' was not found.",
            Status = StatusCodes.Status404NotFound,
            Instance = $"/api/v1/clients/{id}"
        });
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="command">The create client command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created client.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ClientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateClient(
        [FromBody] CreateClientCommand command,
        [FromServices] CreateClientCommandHandler handler,
        [FromServices] IMapper mapper,
        [FromServices] IClientRepository clientRepository,
        [FromServices] IValidator<CreateClientCommand> validator,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "One or more validation errors occurred.",
                Extensions = { ["errors"] = validationResult.ToDictionary() }
            };
            return BadRequest(problemDetails);
        }

        var clientId = await handler.HandleAsync(command, CancellationToken.None);
        var client = await clientRepository.GetByIdAsync(clientId, CancellationToken.None);
        var clientDto = mapper.Map<ClientDto>(client!);
        return Created(new Uri($"/api/v1/clients/{clientId}", UriKind.Relative), clientDto);
    }

    /// <summary>
    /// Updates an existing client.
    /// </summary>
    /// <param name="id">The client ID.</param>
    /// <param name="command">The update client command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateClient(
        Guid id,
        [FromBody] UpdateClientCommand command,
        [FromServices] UpdateClientCommandHandler handler,
        [FromServices] IValidator<UpdateClientCommand> validator,
        CancellationToken ct)
    {
        if (id != command.Id)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "ID mismatch",
                Detail = "Route ID does not match command ID.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "One or more validation errors occurred.",
                Extensions = { ["errors"] = validationResult.ToDictionary() }
            };
            return BadRequest(problemDetails);
        }

        await handler.HandleAsync(command, ct);
        return NoContent();
    }

    /// <summary>
    /// Deactivates a client.
    /// </summary>
    /// <param name="id">The client ID.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateClient(
        Guid id,
        [FromServices] DeactivateClientCommandHandler handler,
        CancellationToken ct)
    {
        await handler.HandleAsync(new DeactivateClientCommand(id), ct);
        return NoContent();
    }

    /// <summary>
    /// Deletes a client.
    /// </summary>
    /// <param name="id">The client ID.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClient(
        Guid id,
        [FromServices] DeleteClientCommandHandler handler,
        CancellationToken ct)
    {
        await handler.HandleAsync(new DeleteClientCommand(id), ct);
        return NoContent();
    }
}