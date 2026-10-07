// <copyright file="SectorRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Sectors.Entities;
using Tfs.Portfolio.Domain.Sectors.Repositories;

/// <summary>
/// Repository implementation for sectors.
/// </summary>
public sealed class SectorRepository : ISectorRepository
{
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SectorRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SectorRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Sectors
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Sector>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Sectors
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Sector>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Sectors
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Sector sector, CancellationToken cancellationToken = default)
    {
        await this.context.Sectors.AddAsync(sector, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Sector sector)
    {
        var entry = this.context.Entry(sector);
        if (entry.State == EntityState.Detached)
        {
            this.context.Sectors.Update(sector);
        }
    }

    /// <inheritdoc />
    public void Delete(Sector sector)
    {
        this.context.Sectors.Remove(sector);
    }
}