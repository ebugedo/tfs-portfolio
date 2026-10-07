// <copyright file="ServiceRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.Domain.Services.Repositories;

/// <summary>
/// Repository implementation for services.
/// </summary>
public sealed class ServiceRepository : IServiceRepository
{
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ServiceRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Services
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Services
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Service>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Services
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Service>> GetByCategoryAsync(ServiceCategory category, CancellationToken cancellationToken = default)
    {
        return await this.context.Services
            .AsNoTracking()
            .Where(s => s.Category == category)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Service service, CancellationToken cancellationToken = default)
    {
        await this.context.Services.AddAsync(service, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Service service)
    {
        var entry = this.context.Entry(service);
        if (entry.State == EntityState.Detached)
        {
            this.context.Services.Update(service);
        }
    }

    /// <inheritdoc />
    public void Delete(Service service)
    {
        this.context.Services.Remove(service);
    }
}