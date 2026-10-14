// <copyright file="ServicesController.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Controllers;

using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Application.Services.Queries;
using Tfs.Portfolio.Application.Services.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Services.Repositories;
using Tfs.Portfolio.Domain.Services.Entities;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing services.
/// </summary>
[ApiController]
[Route("api/v1/services")]
[Tags("Services")]
public sealed class ServicesController : ControllerBase
{
    /// <summary>
    /// Gets all services.
    /// </summary>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of services.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices(
        [FromServices] GetServicesQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetServicesQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets all active services.
    /// </summary>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of active services.</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveServices(
        [FromServices] GetActiveServicesQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetActiveServicesQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets services by category.
    /// </summary>
    /// <param name="category">The service category.</param>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of services in the category.</returns>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServicesByCategory(
        ServiceCategory category,
        [FromServices] GetServicesByCategoryQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetServicesByCategoryQuery(category), ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a service by ID.
    /// </summary>
    /// <param name="id">The service ID.</param>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The service if found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceById(
        Guid id,
        [FromServices] GetServiceByIdQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetServiceByIdQuery(id), ct);
        return result is not null ? Ok(result) : NotFound(new ProblemDetails
        {
            Title = "Not Found",
            Detail = $"Service with ID '{id}' was not found.",
            Status = StatusCodes.Status404NotFound,
            Instance = $"/api/v1/services/{id}"
        });
    }

    /// <summary>
    /// Creates a new service.
    /// </summary>
    /// <param name="command">The create service command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created service.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateService(
        [FromBody] CreateServiceCommand command,
        [FromServices] CreateServiceCommandHandler handler,
        [FromServices] IMapper mapper,
        [FromServices] IServiceRepository serviceRepository,
        [FromServices] IValidator<CreateServiceCommand> validator,
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

        var serviceId = await handler.HandleAsync(command, ct);
        var service = await serviceRepository.GetByIdAsync(serviceId, ct);
        var serviceDto = mapper.Map<ServiceDto>(service!);
        return Created(new Uri($"/api/v1/services/{serviceId}", UriKind.Relative), serviceDto);
    }

    /// <summary>
    /// Updates an existing service.
    /// </summary>
    /// <param name="id">The service ID.</param>
    /// <param name="command">The update service command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateService(
        Guid id,
        [FromBody] UpdateServiceCommand command,
        [FromServices] UpdateServiceCommandHandler handler,
        [FromServices] IValidator<UpdateServiceCommand> validator,
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
    /// Deletes a service.
    /// </summary>
    /// <param name="id">The service ID.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteService(
        Guid id,
        [FromServices] DeleteServiceCommandHandler handler,
        CancellationToken ct)
    {
        await handler.HandleAsync(new DeleteServiceCommand(id), ct);
        return NoContent();
    }
}