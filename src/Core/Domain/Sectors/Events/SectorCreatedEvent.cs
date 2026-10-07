// <copyright file="SectorCreatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Sectors.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a sector is created.
/// </summary>
public sealed record SectorCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the sector identifier.
    /// </summary>
    public Guid SectorId { get; }

    /// <summary>
    /// Gets the sector name.
    /// </summary>
    public string SectorName { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="SectorCreatedEvent"/> class.
    /// </summary>
    /// <param name="sectorId">The sector identifier.</param>
    /// <param name="sectorName">The sector name.</param>
    public SectorCreatedEvent(Guid sectorId, string sectorName)
    {
        this.SectorId = sectorId;
        this.SectorName = sectorName;
    }
}