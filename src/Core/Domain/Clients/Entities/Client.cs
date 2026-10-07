// <copyright file="Client.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Clients.Entities;

using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Clients.Events;
using Tfs.Portfolio.Domain.Clients.Exceptions;

/// <summary>
/// Represents a client aggregate root.
/// </summary>
public sealed class Client : AggregateRoot<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    private Client()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="name">The client name.</param>
    /// <param name="logoUrl">The client logo URL.</param>
    /// <param name="email">The client email.</param>
    /// <param name="phone">The client phone number.</param>
    /// <param name="address">The client address.</param>
    /// <param name="isActive">The active status.</param>
    /// <param name="createdAt">The creation date.</param>
    private Client(Guid id, string name, Url? logoUrl, string email, string? phone, string? address, bool isActive, DateTime createdAt)
        : base(id)
    {
        this.Name = name;
        this.LogoUrl = logoUrl;
        this.Email = email;
        this.Phone = phone;
        this.Address = address;
        this.IsActive = isActive;
        this.CreatedAt = createdAt;
    }

    /// <summary>
    /// Gets the name of the client.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Gets the logo URL of the client.
    /// </summary>
    public Url? LogoUrl { get; private set; }

    /// <summary>
    /// Gets the email of the client.
    /// </summary>
    public string Email { get; private set; } = default!;

    /// <summary>
    /// Gets the phone number of the client.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Gets the address of the client.
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the client is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the date and time when the client was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the client was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="name">The client name.</param>
    /// <param name="email">The client email.</param>
    /// <param name="logoUrl">The client logo URL.</param>
    /// <param name="phone">The client phone number.</param>
    /// <param name="address">The client address.</param>
    /// <returns>A new client instance.</returns>
    /// <exception cref="InvalidClientStateException">Thrown when the name is empty or email is invalid.</exception>
    public static Client Create(string name, string email, Url? logoUrl = null, string? phone = null, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidClientStateException("Client name cannot be empty");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > 200)
        {
            throw new InvalidClientStateException("Client name must not exceed 200 characters");
        }

        var client = new Client(Guid.NewGuid(), trimmedName, logoUrl, email.Trim(), phone?.Trim(), address?.Trim(), true, DateTime.UtcNow);
        client.AddDomainEvent(new ClientCreatedEvent(client.Id, client.Name));
        return client;
    }

    /// <summary>
    /// Updates the client details.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="email">The new email.</param>
    /// <param name="logoUrl">The new logo URL.</param>
    /// <param name="phone">The new phone number.</param>
    /// <param name="address">The new address.</param>
    /// <exception cref="InvalidClientStateException">Thrown when the name is empty or email is invalid.</exception>
    public void Update(string name, string email, Url? logoUrl = null, string? phone = null, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidClientStateException("Client name cannot be empty");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > 200)
        {
            throw new InvalidClientStateException("Client name must not exceed 200 characters");
        }

        this.Name = trimmedName;
        this.Email = email.Trim();
        this.LogoUrl = logoUrl;
        this.Phone = phone?.Trim();
        this.Address = address?.Trim();
        this.UpdatedAt = DateTime.UtcNow;
        this.AddDomainEvent(new ClientUpdatedEvent(this.Id));
    }

    /// <summary>
    /// Deactivates the client.
    /// </summary>
    public void Deactivate()
    {
        this.IsActive = false;
        this.UpdatedAt = DateTime.UtcNow;
        this.AddDomainEvent(new ClientDeactivatedEvent(this.Id));
    }
}