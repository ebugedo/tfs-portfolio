// <copyright file="ServiceCreatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Services.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a service is created.
/// </summary>
public sealed record ServiceCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the service identifier.
    /// </summary>
    public Guid ServiceId { get; }

    /// <summary>
    /// Gets the service name.
    /// </summary>
    public string ServiceName { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceCreatedEvent"/> class.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="serviceName">The service name.</param>
    public ServiceCreatedEvent(Guid serviceId, string serviceName)
    {
        this.ServiceId = serviceId;
        this.ServiceName = serviceName;
    }
}