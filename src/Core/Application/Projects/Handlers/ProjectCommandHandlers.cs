// <copyright file="ProjectCommandHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Handlers;

using AutoMapper;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Exceptions;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.Domain.Clients.Repositories;
using Tfs.Portfolio.Domain.Sectors.Repositories;
using Tfs.Portfolio.Domain.Services.Exceptions;
using Tfs.Portfolio.Domain.Services.Repositories;

/// <summary>
/// Handler for <see cref="CreateProjectCommand"/>.
/// </summary>
public sealed class CreateProjectCommandHandler : CommandHandlerBase<CreateProjectCommand, Guid>
{
    private readonly IProjectRepository projectRepository;
    private readonly IClientRepository clientRepository;
    private readonly ISectorRepository sectorRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="sectorRepository">The sector repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="mapper">The mapper.</param>
    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IClientRepository clientRepository,
        ISectorRepository sectorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        this.projectRepository = projectRepository;
        this.clientRepository = clientRepository;
        this.sectorRepository = sectorRepository;
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<Guid> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var client = await this.clientRepository.GetByIdAsync(command.ClientId, cancellationToken);
        if (client is null)
        {
            throw new InvalidProjectStateException($"Client with ID '{command.ClientId}' was not found");
        }

        var sector = await this.sectorRepository.GetByIdAsync(command.SectorId, cancellationToken);
        if (sector is null)
        {
            throw new InvalidProjectStateException($"Sector with ID '{command.SectorId}' was not found");
        }

        var project = Project.Create(command.Name, command.Description, command.StartDate, command.DurationMonths, command.ClientId, command.SectorId, command.Technologies);
        await this.projectRepository.AddAsync(project, cancellationToken);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
        return project.Id;
    }
}

/// <summary>
/// Handler for <see cref="UpdateProjectCommand"/>.
/// </summary>
public sealed class UpdateProjectCommandHandler : CommandHandlerBase<UpdateProjectCommand>
{
    private readonly IProjectRepository projectRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        this.projectRepository = projectRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(UpdateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await this.projectRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
        {
            throw new ProjectNotFoundException(command.Id);
        }

        // Update basic properties
        project.Update(command.Name, command.Description, command.StartDate, command.DurationMonths, command.ClientId, command.SectorId);

        // Update technologies - clear and re-add
        var currentTechnologies = project.Technologies.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var newTechnologies = command.Technologies?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Remove technologies not in new list
        foreach (var tech in currentTechnologies.Except(newTechnologies))
        {
            project.RemoveTechnology(project.Technologies.First(t => t.Name.Equals(tech, StringComparison.OrdinalIgnoreCase)));
        }

        // Add new technologies
        foreach (var techName in newTechnologies.Except(currentTechnologies))
        {
            if (!string.IsNullOrWhiteSpace(techName))
            {
                project.AddTechnology(Technology.Create(techName.Trim(), TechnologyCategory.Tools, ProficiencyLevel.Intermediate));
            }
        }

        // Update status if changed
        if (project.Status != command.Status)
        {
            project.ChangeStatus(command.Status);
        }

        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="DeleteProjectCommand"/>.
/// </summary>
public sealed class DeleteProjectCommandHandler : CommandHandlerBase<DeleteProjectCommand>
{
    private readonly IProjectRepository projectRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public DeleteProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        this.projectRepository = projectRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await this.projectRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
        {
            throw new ProjectNotFoundException(command.Id);
        }

        this.projectRepository.Delete(project);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="ChangeProjectStatusCommand"/>.
/// </summary>
public sealed class ChangeProjectStatusCommandHandler : CommandHandlerBase<ChangeProjectStatusCommand>
{
    private readonly IProjectRepository projectRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeProjectStatusCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public ChangeProjectStatusCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        this.projectRepository = projectRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(ChangeProjectStatusCommand command, CancellationToken cancellationToken = default)
    {
        var project = await this.projectRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
        {
            throw new ProjectNotFoundException(command.Id);
        }

        project.ChangeStatus(command.Status);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="AddProjectServiceCommand"/>.
/// </summary>
public sealed class AddProjectServiceCommandHandler : CommandHandlerBase<AddProjectServiceCommand>
{
    private readonly IProjectRepository projectRepository;
    private readonly IServiceRepository serviceRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddProjectServiceCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public AddProjectServiceCommandHandler(
        IProjectRepository projectRepository,
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        this.projectRepository = projectRepository;
        this.serviceRepository = serviceRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(AddProjectServiceCommand command, CancellationToken cancellationToken = default)
    {
        var project = await this.projectRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var service = await this.serviceRepository.GetByIdAsync(command.ServiceId, cancellationToken);
        if (service is null)
        {
            throw new ServiceNotFoundException(command.ServiceId);
        }

        project.AddService(command.ServiceId);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="RemoveProjectServiceCommand"/>.
/// </summary>
public sealed class RemoveProjectServiceCommandHandler : CommandHandlerBase<RemoveProjectServiceCommand>
{
    private readonly IProjectRepository projectRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveProjectServiceCommandHandler"/> class.
    /// </summary>
    /// <param name="projectRepository">The project repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public RemoveProjectServiceCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        this.projectRepository = projectRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(RemoveProjectServiceCommand command, CancellationToken cancellationToken = default)
    {
        var project = await this.projectRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        project.RemoveService(command.ServiceId);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
