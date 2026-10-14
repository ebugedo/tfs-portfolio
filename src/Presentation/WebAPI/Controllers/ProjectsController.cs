// <copyright file="ProjectsController.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Controllers;

using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Projects.Queries;
using Tfs.Portfolio.Application.Projects.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.Domain.Projects.Entities;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing projects.
/// </summary>
[ApiController]
[Route("api/v1/projects")]
[Tags("Projects")]
public sealed class ProjectsController : ControllerBase
{
    /// <summary>
    /// Gets all projects with optional filters.
    /// </summary>
    /// <param name="clientId">Optional client ID filter.</param>
    /// <param name="sectorId">Optional sector ID filter.</param>
    /// <param name="technology">Optional technology filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of projects.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects(
        [FromQuery] Guid? clientId,
        [FromQuery] Guid? sectorId,
        [FromQuery] string? technology,
        [FromQuery] ProjectStatus? status,
        [FromServices] GetProjectsQueryHandler handler,
        CancellationToken ct)
    {
        var query = new GetProjectsQuery(clientId, sectorId, technology, status);
        var result = await handler.HandleAsync(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Gets a project by ID.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The project if found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectById(
        Guid id,
        [FromServices] GetProjectByIdQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetProjectByIdQuery(id), ct);
        return result is not null ? Ok(result) : NotFound(new ProblemDetails
        {
            Title = "Not Found",
            Detail = $"Project with ID '{id}' was not found.",
            Status = StatusCodes.Status404NotFound,
            Instance = $"/api/v1/projects/{id}"
        });
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="command">The create project command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created project.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject(
        [FromBody] CreateProjectCommand command,
        [FromServices] CreateProjectCommandHandler handler,
        [FromServices] IMapper mapper,
        [FromServices] IProjectRepository projectRepository,
        [FromServices] IValidator<CreateProjectCommand> validator,
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

        var projectId = await handler.HandleAsync(command, ct);
        var project = await projectRepository.GetByIdAsync(projectId, ct);
        var projectDto = mapper.Map<ProjectDto>(project!);
        return Created(new Uri($"/api/v1/projects/{projectId}", UriKind.Relative), projectDto);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="command">The update project command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(
        Guid id,
        [FromBody] UpdateProjectCommand command,
        [FromServices] UpdateProjectCommandHandler handler,
        [FromServices] IValidator<UpdateProjectCommand> validator,
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
    /// Changes a project's status.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="command">The change project status command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeProjectStatus(
        Guid id,
        [FromBody] ChangeProjectStatusCommand command,
        [FromServices] ChangeProjectStatusCommandHandler handler,
        [FromServices] IValidator<ChangeProjectStatusCommand> validator,
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
    /// Adds a service to a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpPost("{projectId:guid}/services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddProjectService(
        Guid projectId,
        Guid serviceId,
        [FromServices] AddProjectServiceCommandHandler handler,
        [FromServices] IValidator<AddProjectServiceCommand> validator,
        CancellationToken ct)
    {
        var command = new AddProjectServiceCommand(projectId, serviceId);
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
    /// Removes a service from a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{projectId:guid}/services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveProjectService(
        Guid projectId,
        Guid serviceId,
        [FromServices] RemoveProjectServiceCommandHandler handler,
        [FromServices] IValidator<RemoveProjectServiceCommand> validator,
        CancellationToken ct)
    {
        var command = new RemoveProjectServiceCommand(projectId, serviceId);
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
    /// Deletes a project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(
        Guid id,
        [FromServices] DeleteProjectCommandHandler handler,
        CancellationToken ct)
    {
        await handler.HandleAsync(new DeleteProjectCommand(id), ct);
        return NoContent();
    }
}