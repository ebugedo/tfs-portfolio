// <copyright file="ClientRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Clients.Repositories;

/// <summary>
/// Repository implementation for clients.
/// </summary>
public sealed class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ClientRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Clients
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Clients
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Client>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Clients
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
    {
        await this.context.Clients.AddAsync(client, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Client client)
    {
        var entry = this.context.Entry(client);
        if (entry.State == EntityState.Detached)
        {
            this.context.Clients.Update(client);
        }
    }

    /// <inheritdoc />
    public void Delete(Client client)
    {
        this.context.Clients.Remove(client);
    }
}