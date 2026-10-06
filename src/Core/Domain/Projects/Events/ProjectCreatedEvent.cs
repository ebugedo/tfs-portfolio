// <copyright file="ProjectCreatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a project is created.
/// </summary>
public sealed record ProjectCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the project identifier.
    /// </summary>
    public Guid ProjectId { get; }

    /// <summary>
    /// Gets the project name.
    /// </summary>
    public string ProjectName { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectCreatedEvent"/> class.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="projectName">The project name.</param>
    public ProjectCreatedEvent(Guid projectId, string projectName)
    {
        this.ProjectId = projectId;
        this.ProjectName = projectName;
    }
}