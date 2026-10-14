// <copyright file="SectorsController.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Controllers;

using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Application.Sectors.Dtos;
using Tfs.Portfolio.Application.Sectors.Queries;
using Tfs.Portfolio.Application.Sectors.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Sectors.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing sectors.
/// </summary>
[ApiController]
[Route("api/v1/sectors")]
[Tags("Sectors")]
public sealed class SectorsController : ControllerBase
{
    /// <summary>
    /// Gets all sectors.
    /// </summary>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of sectors.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SectorListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSectors(
        [FromServices] GetSectorsQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetSectorsQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets all active sectors.
    /// </summary>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of active sectors.</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<SectorListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveSectors(
        [FromServices] GetActiveSectorsQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetActiveSectorsQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a sector by ID.
    /// </summary>
    /// <param name="id">The sector ID.</param>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The sector if found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSectorById(
        Guid id,
        [FromServices] GetSectorByIdQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetSectorByIdQuery(id), ct);
        return result is not null ? Ok(result) : NotFound(new ProblemDetails
        {
            Title = "Not Found",
            Detail = $"Sector with ID '{id}' was not found.",
            Status = StatusCodes.Status404NotFound,
            Instance = $"/api/v1/sectors/{id}"
        });
    }

    /// <summary>
    /// Creates a new sector.
    /// </summary>
    /// <param name="command">The create sector command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="sectorRepository">The sector repository.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created sector.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SectorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSector(
        [FromBody] CreateSectorCommand command,
        [FromServices] CreateSectorCommandHandler handler,
        [FromServices] IMapper mapper,
        [FromServices] ISectorRepository sectorRepository,
        [FromServices] IValidator<CreateSectorCommand> validator,
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

        var sectorId = await handler.HandleAsync(command, ct);
        var sector = await sectorRepository.GetByIdAsync(sectorId, ct);
        var sectorDto = mapper.Map<SectorDto>(sector!);
        return Created(new Uri($"/api/v1/sectors/{sectorId}", UriKind.Relative), sectorDto);
    }

    /// <summary>
    /// Updates an existing sector.
    /// </summary>
    /// <param name="id">The sector ID.</param>
    /// <param name="command">The update sector command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSector(
        Guid id,
        [FromBody] UpdateSectorCommand command,
        [FromServices] UpdateSectorCommandHandler handler,
        [FromServices] IValidator<UpdateSectorCommand> validator,
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
    /// Deletes a sector.
    /// </summary>
    /// <param name="id">The sector ID.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSector(
        Guid id,
        [FromServices] DeleteSectorCommandHandler handler,
        CancellationToken ct)
    {
        await handler.HandleAsync(new DeleteSectorCommand(id), ct);
        return NoContent();
    }
}