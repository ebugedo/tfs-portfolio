// <copyright file="ProjectEndpoints.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Endpoints;

using Microsoft.AspNetCore.Mvc;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Projects.Queries;
using Tfs.Portfolio.Application.Projects.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Repositories;
using AutoMapper;
using Tfs.Portfolio.Api.Filters;

/// <summary>
/// Project endpoints.
/// </summary>
internal static class ProjectEndpoints
{
    /// <summary>
    /// Maps the project endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/projects").WithTags("Projects");

        group.MapGet("/", async (
            [FromServices] GetProjectsQueryHandler getAllHandler,
            [FromQuery] Guid? clientId,
            [FromQuery] Guid? sectorId,
            [FromQuery] string? technology,
            [FromQuery] ProjectStatus? status,
            CancellationToken ct) =>
        {
            var query = new GetProjectsQuery(clientId, sectorId, technology, status);
            var result = await getAllHandler.HandleAsync(query, ct);
            return Results.Ok(result);
        })
        .WithName("GetProjects")
        .Produces<IReadOnlyList<ProjectListItemDto>>();

        group.MapGet("/{id:guid}", async (Guid id, [FromServices] GetProjectByIdQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetProjectByIdQuery(id), ct);
            return result is not null ? Results.Ok(result) : Results.Problem(
                title: "Not Found",
                detail: $"Project with ID '{id}' was not found.",
                statusCode: 404,
                instance: $"/api/v1/projects/{id}");
        })
        .WithName("GetProjectById")
        .Produces<ProjectDto>()
        .Produces(404);

        group.MapPost("/", async (CreateProjectCommand command, [FromServices] CreateProjectCommandHandler handler, [FromServices] IMapper mapper, [FromServices] IProjectRepository projectRepository, CancellationToken ct) =>
        {
            var projectId = await handler.HandleAsync(command, ct);
            var project = await projectRepository.GetByIdAsync(projectId, ct);
            var projectDto = mapper.Map<ProjectDto>(project!);
            return Results.Created($"/api/v1/projects/{projectId}", projectDto);
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

        group.MapPatch("/{id:guid}/status", async (Guid id, [FromBody] ChangeProjectStatusCommand command, [FromServices] ChangeProjectStatusCommandHandler handler, CancellationToken ct) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest(new ProblemDetails { Title = "ID mismatch", Detail = "Route ID does not match command ID." });
            }
            await handler.HandleAsync(command, ct);
            return Results.NoContent();
        })
        .WithName("ChangeProjectStatus")
        .Accepts<ChangeProjectStatusCommand>("application/json")
        .Produces(204)
        .Produces<ProblemDetails>(400)
        .Produces(404)
        .AddEndpointFilter<ValidationFilter<ChangeProjectStatusCommand>>();

        group.MapPost("/{projectId:guid}/services/{serviceId:guid}", async (Guid projectId, Guid serviceId, [FromServices] AddProjectServiceCommandHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new AddProjectServiceCommand(projectId, serviceId), ct);
            return Results.NoContent();
        })
        .WithName("AddProjectService")
        .Produces(204)
        .Produces(404);

        group.MapDelete("/{projectId:guid}/services/{serviceId:guid}", async (Guid projectId, Guid serviceId, [FromServices] RemoveProjectServiceCommandHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(new RemoveProjectServiceCommand(projectId, serviceId), ct);
            return Results.NoContent();
        })
        .WithName("RemoveProjectService")
        .Produces(204)
        .Produces(404);

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