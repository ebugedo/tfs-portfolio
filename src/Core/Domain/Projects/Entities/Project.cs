// <copyright file="Project.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Entities;

using Tfs.Portfolio.Domain.Common;
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
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Project"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    /// <param name="createdAt">The creation date.</param>
    private Project(Guid id, string name, string? description, DateTime createdAt)
        : base(id)
    {
        this.Name = name;
        this.Description = description;
        this.CreatedAt = createdAt;
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
    /// <returns>A new project instance.</returns>
    /// <exception cref="InvalidProjectStateException">Thrown when the name is empty.</exception>
    public static Project Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidProjectStateException("Project name cannot be empty");
        }

        var project = new Project(Guid.NewGuid(), name.Trim(), description?.Trim(), DateTime.UtcNow);
        project.AddDomainEvent(new ProjectCreatedEvent(project.Id, project.Name));
        return project;
    }

    /// <summary>
    /// Updates the project details.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="description">The new description.</param>
    /// <exception cref="InvalidProjectStateException">Thrown when the name is empty.</exception>
    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidProjectStateException("Project name cannot be empty");
        }

        this.Name = name.Trim();
        this.Description = description?.Trim();
        this.UpdatedAt = DateTime.UtcNow;
    }
}
