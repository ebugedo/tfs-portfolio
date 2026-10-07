// <copyright file="Service.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Services.Entities;

using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Services.Events;
using Tfs.Portfolio.Domain.Services.Exceptions;

/// <summary>
/// Represents a service aggregate root offered by the company.
/// </summary>
public sealed class Service : AggregateRoot<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Service"/> class.
    /// </summary>
    private Service()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Service"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="name">The service name.</param>
    /// <param name="description">The service description.</param>
    /// <param name="category">The service category.</param>
    /// <param name="isActive">The active status.</param>
    /// <param name="createdAt">The creation date.</param>
    private Service(Guid id, string name, string? description, ServiceCategory category, bool isActive, DateTime createdAt)
        : base(id)
    {
        this.Name = name;
        this.Description = description;
        this.Category = category;
        this.IsActive = isActive;
        this.CreatedAt = createdAt;
    }

    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Gets the description of the service.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the category of the service.
    /// </summary>
    public ServiceCategory Category { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the service is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the date and time when the service was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the service was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new service.
    /// </summary>
    /// <param name="name">The service name.</param>
    /// <param name="description">The service description.</param>
    /// <param name="category">The service category.</param>
    /// <returns>A new service instance.</returns>
    /// <exception cref="InvalidServiceStateException">Thrown when the name is empty or exceeds max length.</exception>
    public static Service Create(string name, string? description, ServiceCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidServiceStateException("Service name cannot be empty");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > 150)
        {
            throw new InvalidServiceStateException("Service name must not exceed 150 characters");
        }

        var trimmedDescription = description?.Trim();
        if (trimmedDescription is { Length: > 1000 })
        {
            throw new InvalidServiceStateException("Service description must not exceed 1000 characters");
        }

        var service = new Service(Guid.NewGuid(), trimmedName, trimmedDescription, category, true, DateTime.UtcNow);
        service.AddDomainEvent(new ServiceCreatedEvent(service.Id, service.Name));
        return service;
    }

    /// <summary>
    /// Updates the service details.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="description">The new description.</param>
    /// <param name="category">The new category.</param>
    /// <exception cref="InvalidServiceStateException">Thrown when the name is empty or exceeds max length.</exception>
    public void Update(string name, string? description, ServiceCategory category)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidServiceStateException("Service name cannot be empty");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > 150)
        {
            throw new InvalidServiceStateException("Service name must not exceed 150 characters");
        }

        var trimmedDescription = description?.Trim();
        if (trimmedDescription is { Length: > 1000 })
        {
            throw new InvalidServiceStateException("Service description must not exceed 1000 characters");
        }

        this.Name = trimmedName;
        this.Description = trimmedDescription;
        this.Category = category;
        this.UpdatedAt = DateTime.UtcNow;
        this.AddDomainEvent(new ServiceUpdatedEvent(this.Id));
    }
}