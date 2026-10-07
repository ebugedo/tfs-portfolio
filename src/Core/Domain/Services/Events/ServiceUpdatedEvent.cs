// <copyright file="ServiceUpdatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Services.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a service is updated.
/// </summary>
public sealed record ServiceUpdatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the service identifier.
    /// </summary>
    public Guid ServiceId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUpdatedEvent"/> class.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    public ServiceUpdatedEvent(Guid serviceId)
    {
        this.ServiceId = serviceId;
    }
}