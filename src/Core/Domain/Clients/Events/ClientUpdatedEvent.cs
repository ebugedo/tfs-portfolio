// <copyright file="ClientUpdatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Clients.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a client is updated.
/// </summary>
public sealed record ClientUpdatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the client identifier.
    /// </summary>
    public Guid ClientId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientUpdatedEvent"/> class.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    public ClientUpdatedEvent(Guid clientId)
    {
        this.ClientId = clientId;
    }
}