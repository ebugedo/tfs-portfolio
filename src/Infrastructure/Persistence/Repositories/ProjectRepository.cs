// <copyright file="ProjectRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Npgsql;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Projects.Repositories;

/// <summary>
/// Repository implementation for projects.
/// </summary>
public sealed class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ProjectRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Projects
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Projects
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Project>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Projects
            .AsNoTracking()
            .Where(p => p.Status == ProjectStatus.Active)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Project>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        return await this.context.Projects
            .AsNoTracking()
            .Where(p => p.ClientId == clientId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Project>> GetBySectorIdAsync(Guid sectorId, CancellationToken cancellationToken = default)
    {
        return await this.context.Projects
            .AsNoTracking()
            .Where(p => p.SectorId == sectorId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Project>> GetByTechnologyAsync(string technologyName, CancellationToken cancellationToken = default)
    {
        var sql = @"
            SELECT p.* FROM ""Projects"" p
            WHERE EXISTS (
                SELECT 1 FROM jsonb_array_elements(p.""Technologies"") AS tech
                WHERE LOWER(tech->>'Name') = LOWER(@technologyName)
            )
            ORDER BY p.""Name""";

        return await this.context.Projects
            .FromSqlRaw(sql, new NpgsqlParameter("technologyName", technologyName))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        return await this.context.Projects
            .AsNoTracking()
            .Where(p => p.Status == status)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Project>> GetFilteredAsync(
        Guid? clientId = null,
        Guid? sectorId = null,
        string? technologyName = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = this.context.Projects.AsNoTracking();

        if (clientId.HasValue)
        {
            query = query.Where(p => p.ClientId == clientId.Value);
        }

        if (sectorId.HasValue)
        {
            query = query.Where(p => p.SectorId == sectorId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        var projects = await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(technologyName))
        {
            // Filter by technology name in memory since JSONB array filtering is not translatable
            projects = projects.Where(p => p.Technologies.Any(t => t.Name.Equals(technologyName, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        return projects;
    }

    /// <inheritdoc />
    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await this.context.Projects.AddAsync(project, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Project project)
    {
        var entry = this.context.Entry(project);
        if (entry.State == EntityState.Detached)
        {
            this.context.Projects.Update(project);
        }

        // If already tracked, change tracker auto-detects modifications
    }

    /// <inheritdoc />
    public void Delete(Project project)
    {
        this.context.Projects.Remove(project);
    }
}