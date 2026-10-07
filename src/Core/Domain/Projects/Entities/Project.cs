// <copyright file="Project.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Entities;

using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Projects.Events;
using Tfs.Portfolio.Domain.Projects.Exceptions;

/// <summary>
/// Represents a project aggregate root.
/// </summary>
public sealed class Project : AggregateRoot<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Project"/> class.
    /// </summary>
    private Project()
    {
        this.Technologies = Array.Empty<Technology>();
        this.ServiceIds = Array.Empty<Guid>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Project"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    /// <param name="startDate">The project start date.</param>
    /// <param name="durationMonths">The project duration in months.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="sectorId">The sector identifier.</param>
    /// <param name="createdAt">The creation date.</param>
    private Project(Guid id, string name, string? description, YearMonth startDate, int durationMonths, Guid clientId, Guid sectorId, DateTime createdAt)
        : base(id)
    {
        this.Name = name;
        this.Description = description;
        this.StartDate = startDate;
        this.DurationMonths = durationMonths;
        this.ClientId = clientId;
        this.SectorId = sectorId;
        this.Status = ProjectStatus.Draft;
        this.CreatedAt = createdAt;
        this.Technologies = Array.Empty<Technology>();
        this.ServiceIds = Array.Empty<Guid>();
    }

    /// <summary>
    /// Gets the name of the project.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Gets the description of the project.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the project start date.
    /// </summary>
    public YearMonth StartDate { get; private set; }

    /// <summary>
    /// Gets the project duration in months.
    /// </summary>
    public int DurationMonths { get; private set; }

    /// <summary>
    /// Gets the computed project end date.
    /// </summary>
    public YearMonth EndDate => this.StartDate.AddMonths(this.DurationMonths);

    /// <summary>
    /// Gets the project technologies.
    /// </summary>
    public IReadOnlyList<Technology> Technologies { get; private set; }

    /// <summary>
    /// Gets the client identifier.
    /// </summary>
    public Guid ClientId { get; private set; }

    /// <summary>
    /// Gets the sector identifier.
    /// </summary>
    public Guid SectorId { get; private set; }

    /// <summary>
    /// Gets the project status.
    /// </summary>
    public ProjectStatus Status { get; private set; }

    /// <summary>
    /// Gets the associated service identifiers.
    /// </summary>
    public IReadOnlyList<Guid> ServiceIds { get; private set; }

    /// <summary>
    /// Gets the date and time when the project was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the project was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    /// <param name="startDate">The project start date.</param>
    /// <param name="durationMonths">The project duration in months (1-120).</param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="sectorId">The sector identifier.</param>
    /// <returns>A new project instance.</returns>
    /// <exception cref="InvalidProjectStateException">Thrown when validation fails.</exception>
    public static Project Create(string name, string? description, YearMonth startDate, int durationMonths, Guid clientId, Guid sectorId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidProjectStateException("Project name cannot be empty");
        }

        if (durationMonths < 1 || durationMonths > 120)
        {
            throw new InvalidProjectStateException("Duration must be between 1 and 120 months");
        }

        if (clientId == Guid.Empty)
        {
            throw new InvalidProjectStateException("Client not found");
        }

        if (sectorId == Guid.Empty)
        {
            throw new InvalidProjectStateException("Sector not found");
        }

        var project = new Project(Guid.NewGuid(), name.Trim(), description?.Trim(), startDate, durationMonths, clientId, sectorId, DateTime.UtcNow);
        project.AddDomainEvent(new ProjectCreatedEvent(project.Id, project.Name));
        return project;
    }

    /// <summary>
    /// Updates the project details.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="description">The new description.</param>
    /// <param name="startDate">The new start date.</param>
    /// <param name="durationMonths">The new duration in months.</param>
    /// <param name="clientId">The new client identifier.</param>
    /// <param name="sectorId">The new sector identifier.</param>
    /// <exception cref="InvalidProjectStateException">Thrown when validation fails.</exception>
    public void Update(string name, string? description, YearMonth startDate, int durationMonths, Guid clientId, Guid sectorId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidProjectStateException("Project name cannot be empty");
        }

        if (durationMonths < 1 || durationMonths > 120)
        {
            throw new InvalidProjectStateException("Duration must be between 1 and 120 months");
        }

        if (clientId == Guid.Empty)
        {
            throw new InvalidProjectStateException("Client not found");
        }

        if (sectorId == Guid.Empty)
        {
            throw new InvalidProjectStateException("Sector not found");
        }

        this.Name = name.Trim();
        this.Description = description?.Trim();
        this.StartDate = startDate;
        this.DurationMonths = durationMonths;
        this.ClientId = clientId;
        this.SectorId = sectorId;
        this.UpdatedAt = DateTime.UtcNow;
        this.AddDomainEvent(new ProjectUpdatedEvent(this.Id));
    }

    /// <summary>
    /// Changes the project status.
    /// </summary>
    /// <param name="newStatus">The new status.</param>
    /// <exception cref="InvalidProjectStateException">Thrown when the status transition is invalid.</exception>
    public void ChangeStatus(ProjectStatus newStatus)
    {
        var oldStatus = this.Status;

        if (!IsValidTransition(oldStatus, newStatus))
        {
            throw new InvalidProjectStateException($"Invalid status transition from {oldStatus} to {newStatus}");
        }

        this.Status = newStatus;
        this.UpdatedAt = DateTime.UtcNow;
        this.AddDomainEvent(new ProjectStatusChangedEvent(this.Id, oldStatus, newStatus));
    }

    /// <summary>
    /// Adds a technology to the project.
    /// </summary>
    /// <param name="technology">The technology to add.</param>
    /// <exception cref="InvalidProjectStateException">Thrown when the technology already exists.</exception>
    public void AddTechnology(Technology technology)
    {
        if (this.Technologies.Any(t => t.Equals(technology)))
        {
            throw new InvalidProjectStateException("Technology already exists in project");
        }

        var technologies = this.Technologies.ToList();
        technologies.Add(technology);
        this.Technologies = technologies;
        this.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes a technology from the project.
    /// </summary>
    /// <param name="technology">The technology to remove.</param>
    public void RemoveTechnology(Technology technology)
    {
        var technologies = this.Technologies.Where(t => !t.Equals(technology)).ToList();
        this.Technologies = technologies;
        this.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a service to the project.
    /// </summary>
    /// <param name="serviceId">The service identifier to add.</param>
    /// <exception cref="InvalidProjectStateException">Thrown when the service is already associated.</exception>
    public void AddService(Guid serviceId)
    {
        if (this.ServiceIds.Contains(serviceId))
        {
            throw new InvalidProjectStateException("Service already associated with project");
        }

        var serviceIds = this.ServiceIds.ToList();
        serviceIds.Add(serviceId);
        this.ServiceIds = serviceIds;
        this.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes a service from the project.
    /// </summary>
    /// <param name="serviceId">The service identifier to remove.</param>
    public void RemoveService(Guid serviceId)
    {
        var serviceIds = this.ServiceIds.Where(id => id != serviceId).ToList();
        this.ServiceIds = serviceIds;
        this.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Determines if a status transition is valid.
    /// </summary>
    /// <param name="from">The current status.</param>
    /// <param name="to">The target status.</param>
    /// <returns>True if the transition is valid; otherwise, false.</returns>
    private static bool IsValidTransition(ProjectStatus from, ProjectStatus to)
    {
        return (from, to) switch
        {
            (ProjectStatus.Draft, ProjectStatus.Active) => true,
            (ProjectStatus.Active, ProjectStatus.OnHold) => true,
            (ProjectStatus.Active, ProjectStatus.Completed) => true,
            (ProjectStatus.Active, ProjectStatus.Cancelled) => true,
            (ProjectStatus.OnHold, ProjectStatus.Active) => true,
            (ProjectStatus.OnHold, ProjectStatus.Cancelled) => true,
            _ => false,
        };
    }
}