// <copyright file="ClientCreatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Clients.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a client is created.
/// </summary>
public sealed record ClientCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the client identifier.
    /// </summary>
    public Guid ClientId { get; }

    /// <summary>
    /// Gets the client name.
    /// </summary>
    public string ClientName { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientCreatedEvent"/> class.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="clientName">The client name.</param>
    public ClientCreatedEvent(Guid clientId, string clientName)
    {
        this.ClientId = clientId;
        this.ClientName = clientName;
    }
}