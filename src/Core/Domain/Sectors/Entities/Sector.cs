// <copyright file="Sector.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Sectors.Entities;

using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Sectors.Events;
using Tfs.Portfolio.Domain.Sectors.Exceptions;

/// <summary>
/// Represents a sector aggregate root for categorizing projects.
/// </summary>
public sealed class Sector : AggregateRoot<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Sector"/> class.
    /// </summary>
    private Sector()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sector"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="name">The sector name.</param>
    /// <param name="description">The sector description.</param>
    /// <param name="isActive">The active status.</param>
    /// <param name="createdAt">The creation date.</param>
    private Sector(Guid id, string name, string? description, bool isActive, DateTime createdAt)
        : base(id)
    {
        this.Name = name;
        this.Description = description;
        this.IsActive = isActive;
        this.CreatedAt = createdAt;
    }

    /// <summary>
    /// Gets the name of the sector.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Gets the description of the sector.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the sector is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the date and time when the sector was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the sector was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new sector.
    /// </summary>
    /// <param name="name">The sector name.</param>
    /// <param name="description">The sector description.</param>
    /// <returns>A new sector instance.</returns>
    /// <exception cref="InvalidSectorStateException">Thrown when the name is empty or exceeds max length.</exception>
    public static Sector Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidSectorStateException("Sector name cannot be empty");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > 100)
        {
            throw new InvalidSectorStateException("Sector name must not exceed 100 characters");
        }

        var trimmedDescription = description?.Trim();
        if (trimmedDescription is { Length: > 500 })
        {
            throw new InvalidSectorStateException("Sector description must not exceed 500 characters");
        }

        var sector = new Sector(Guid.NewGuid(), trimmedName, trimmedDescription, true, DateTime.UtcNow);
        sector.AddDomainEvent(new SectorCreatedEvent(sector.Id, sector.Name));
        return sector;
    }

    /// <summary>
    /// Updates the sector details.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="description">The new description.</param>
    /// <exception cref="InvalidSectorStateException">Thrown when the name is empty or exceeds max length.</exception>
    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidSectorStateException("Sector name cannot be empty");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > 100)
        {
            throw new InvalidSectorStateException("Sector name must not exceed 100 characters");
        }

        var trimmedDescription = description?.Trim();
        if (trimmedDescription is { Length: > 500 })
        {
            throw new InvalidSectorStateException("Sector description must not exceed 500 characters");
        }

        this.Name = trimmedName;
        this.Description = trimmedDescription;
        this.UpdatedAt = DateTime.UtcNow;
        this.AddDomainEvent(new SectorUpdatedEvent(this.Id));
    }
}