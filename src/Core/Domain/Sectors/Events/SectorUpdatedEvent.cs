// <copyright file="SectorUpdatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Sectors.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a sector is updated.
/// </summary>
public sealed record SectorUpdatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the sector identifier.
    /// </summary>
    public Guid SectorId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="SectorUpdatedEvent"/> class.
    /// </summary>
    /// <param name="sectorId">The sector identifier.</param>
    public SectorUpdatedEvent(Guid sectorId)
    {
        this.SectorId = sectorId;
    }
}