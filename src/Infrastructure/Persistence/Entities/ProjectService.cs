// <copyright file="ProjectService.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Entities;

/// <summary>
/// Join entity for Project-Service many-to-many relationship.
/// </summary>
public sealed record ProjectService
{
    /// <summary>
    /// Gets the project identifier.
    /// </summary>
    public Guid ProjectId { get; init; }

    /// <summary>
    /// Gets the service identifier.
    /// </summary>
    public Guid ServiceId { get; init; }
}