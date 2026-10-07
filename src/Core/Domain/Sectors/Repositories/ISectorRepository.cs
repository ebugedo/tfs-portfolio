// <copyright file="ISectorRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Sectors.Repositories;

using Tfs.Portfolio.Domain.Sectors.Entities;

/// <summary>
/// Repository interface for sectors.
/// </summary>
public interface ISectorRepository
{
    /// <summary>
    /// Gets a sector by its identifier.
    /// </summary>
    /// <param name="id">The sector identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The sector if found; otherwise, null.</returns>
    Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all sectors.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of sectors.</returns>
    Task<IReadOnlyList<Sector>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active sectors.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of active sectors.</returns>
    Task<IReadOnlyList<Sector>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new sector.
    /// </summary>
    /// <param name="sector">The sector to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Sector sector, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sector.
    /// </summary>
    /// <param name="sector">The sector to update.</param>
    void Update(Sector sector);

    /// <summary>
    /// Deletes a sector.
    /// </summary>
    /// <param name="sector">The sector to delete.</param>
    void Delete(Sector sector);
}