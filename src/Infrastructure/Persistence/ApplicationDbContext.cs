// <copyright file="ApplicationDbContext.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Sectors.Entities;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.Infrastructure.Persistence.Entities;

/// <summary>
/// Application database context.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the clients.
    /// </summary>
    public DbSet<Client> Clients { get; set; } = default!;

    /// <summary>
    /// Gets or sets the sectors.
    /// </summary>
    public DbSet<Sector> Sectors { get; set; } = default!;

    /// <summary>
    /// Gets or sets the services.
    /// </summary>
    public DbSet<Service> Services { get; set; } = default!;

    /// <summary>
    /// Gets or sets the company profiles.
    /// </summary>
    public DbSet<CompanyProfileEntity> CompanyProfiles { get; set; } = default!;

    /// <summary>
    /// Gets or sets the projects.
    /// </summary>
    public DbSet<Project> Projects { get; set; } = default!;

    /// <summary>
    /// Gets or sets the project-service associations.
    /// </summary>
    public DbSet<ProjectService> ProjectServices { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}