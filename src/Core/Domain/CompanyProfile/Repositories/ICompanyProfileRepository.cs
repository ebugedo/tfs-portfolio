// <copyright file="ICompanyProfileRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.CompanyProfile.Repositories;

using Tfs.Portfolio.Domain.CompanyProfile.Entities;

/// <summary>
/// Repository interface for company profile (singleton).
/// </summary>
public interface ICompanyProfileRepository
{
    /// <summary>
    /// Gets the company profile (singleton).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The company profile if exists; otherwise, null.</returns>
    Task<CompanyProfileEntity?> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new company profile (singleton).
    /// </summary>
    /// <param name="profile">The company profile to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a profile already exists.</exception>
    Task AddAsync(CompanyProfileEntity profile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the company profile.
    /// </summary>
    /// <param name="profile">The company profile to update.</param>
    void Update(CompanyProfileEntity profile);

    /// <summary>
    /// Deletes the company profile.
    /// </summary>
    /// <param name="profile">The company profile to delete.</param>
    void Delete(CompanyProfileEntity profile);
}