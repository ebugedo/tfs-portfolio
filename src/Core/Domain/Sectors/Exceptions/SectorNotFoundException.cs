// <copyright file="SectorNotFoundException.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Sectors.Exceptions;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Exception thrown when a sector is not found.
/// </summary>
public sealed class SectorNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SectorNotFoundException"/> class.
    /// </summary>
    public SectorNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SectorNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public SectorNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SectorNotFoundException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public SectorNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SectorNotFoundException"/> class with the sector identifier.
    /// </summary>
    /// <param name="sectorId">The sector identifier that was not found.</param>
    public SectorNotFoundException(Guid sectorId)
        : base($"Sector with ID '{sectorId}' was not found")
    {
        this.SectorId = sectorId;
    }

    /// <summary>
    /// Gets the sector identifier that was not found.
    /// </summary>
    public Guid SectorId { get; }
}