// <copyright file="CompanyProfileRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Domain.CompanyProfile.Repositories;

/// <summary>
/// Repository implementation for company profile (singleton).
/// </summary>
public sealed class CompanyProfileRepository : ICompanyProfileRepository
{
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CompanyProfileRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<CompanyProfileEntity?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.CompanyProfiles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(CompanyProfileEntity profile, CancellationToken cancellationToken = default)
    {
        var existing = await this.context.CompanyProfiles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
        {
            throw new InvalidOperationException("Company profile already exists");
        }

        await this.context.CompanyProfiles.AddAsync(profile, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(CompanyProfileEntity profile)
    {
        var entry = this.context.Entry(profile);
        if (entry.State == EntityState.Detached)
        {
            this.context.CompanyProfiles.Update(profile);
        }
    }

    /// <inheritdoc />
    public void Delete(CompanyProfileEntity profile)
    {
        this.context.CompanyProfiles.Remove(profile);
    }
}