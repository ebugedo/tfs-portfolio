// <copyright file="ProjectQueryHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Handlers;

using AutoMapper;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Projects.Queries;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Repositories;

/// <summary>
/// Handler for <see cref="GetProjectByIdQuery"/>.
/// </summary>
public sealed class GetProjectByIdQueryHandler : QueryHandlerBase<GetProjectByIdQuery, ProjectDto>
{
    private readonly IProjectRepository projectRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetProjectByIdQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper)
    {
        this.projectRepository = projectRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<ProjectDto> HandleAsync(GetProjectByIdQuery query, CancellationToken cancellationToken = default)
    {
        var project = await this.projectRepository.GetByIdAsync(query.Id, cancellationToken);
        return this.mapper.Map<ProjectDto>(project);
    }
}

/// <summary>
/// Handler for <see cref="GetProjectsQuery"/>.
/// </summary>
public sealed class GetProjectsQueryHandler : QueryHandlerBase<GetProjectsQuery, IReadOnlyList<ProjectListItemDto>>
{
    private readonly IProjectRepository projectRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsQueryHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetProjectsQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper)
    {
        this.projectRepository = projectRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<ProjectListItemDto>> HandleAsync(GetProjectsQuery query, CancellationToken cancellationToken = default)
    {
        var projects = await this.projectRepository.GetAllAsync(cancellationToken);
        return this.mapper.Map<IReadOnlyList<ProjectListItemDto>>(projects);
    }
}