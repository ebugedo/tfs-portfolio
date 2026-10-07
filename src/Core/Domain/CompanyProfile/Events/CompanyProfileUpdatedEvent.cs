// <copyright file="CompanyProfileUpdatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.CompanyProfile.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a company profile is updated.
/// </summary>
public sealed record CompanyProfileUpdatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the profile identifier.
    /// </summary>
    public Guid ProfileId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileUpdatedEvent"/> class.
    /// </summary>
    /// <param name="profileId">The profile identifier.</param>
    public CompanyProfileUpdatedEvent(Guid profileId)
    {
        this.ProfileId = profileId;
    }
}