// <copyright file="CompanyProfileCreatedEvent.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.CompanyProfile.Events;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Event raised when a company profile is created.
/// </summary>
public sealed record CompanyProfileCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the profile identifier.
    /// </summary>
    public Guid ProfileId { get; }

    /// <summary>
    /// Gets the company name.
    /// </summary>
    public string CompanyName { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileCreatedEvent"/> class.
    /// </summary>
    /// <param name="profileId">The profile identifier.</param>
    /// <param name="companyName">The company name.</param>
    public CompanyProfileCreatedEvent(Guid profileId, string companyName)
    {
        this.ProfileId = profileId;
        this.CompanyName = companyName;
    }
}