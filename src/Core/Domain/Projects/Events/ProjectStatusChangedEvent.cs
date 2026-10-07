// <copyright file="ProjectStatusChangedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Events;

using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Event raised when a project status changes.
/// </summary>
public sealed record ProjectStatusChangedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the project identifier.
    /// </summary>
    public Guid ProjectId { get; }

    /// <summary>
    /// Gets the previous status.
    /// </summary>
    public ProjectStatus OldStatus { get; }

    /// <summary>
    /// Gets the new status.
    /// </summary>
    public ProjectStatus NewStatus { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectStatusChangedEvent"/> class.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="oldStatus">The previous status.</param>
    /// <param name="newStatus">The new status.</param>
    public ProjectStatusChangedEvent(Guid projectId, ProjectStatus oldStatus, ProjectStatus newStatus)
    {
        this.ProjectId = projectId;
        this.OldStatus = oldStatus;
        this.NewStatus = newStatus;
    }
}