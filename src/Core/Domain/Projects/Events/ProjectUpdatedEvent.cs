// <copyright file="ProjectUpdatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a project is updated.
/// </summary>
public sealed record ProjectUpdatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the project identifier.
    /// </summary>
    public Guid ProjectId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectUpdatedEvent"/> class.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    public ProjectUpdatedEvent(Guid projectId)
    {
        this.ProjectId = projectId;
    }
}