// <copyright file="CustomWebApplicationFactory.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Testcontainers.PostgreSql;
using Tfs.Portfolio.Api;
using Tfs.Portfolio.Infrastructure.Persistence;
using Xunit;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// Uses Testcontainers PostgreSQL for isolated test database.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202", Justification = "Protected methods before public for logical grouping")]
[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1010", Justification = "Array syntax preference")]
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private Respawner? _respawner;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomWebApplicationFactory"/> class.
    /// </summary>
    public CustomWebApplicationFactory()
    {
        _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("portfolio_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    /// <summary>
    /// Gets the connection string for the test database.
    /// </summary>
    public string ConnectionString => _postgresContainer.GetConnectionString();

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.Testing.json", optional: true, reloadOnChange: true);
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            // Remove the existing ApplicationDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Remove the existing ApplicationDbContext registration (scoped)
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ApplicationDbContext));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Add ApplicationDbContext with test container connection string
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(ConnectionString);
            });

            // Ensure the database is created and migrations are applied
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        });

        builder.UseEnvironment("Testing");
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        // Start the PostgreSQL container
        await _postgresContainer.StartAsync();

        // Apply migrations immediately after container starts, before Respawn initialization
        await ApplyMigrationsAsync();

        // Initialize Respawn for database reset between tests
        await InitializeRespawnerAsync();
    }

    /// <summary>
    /// Applies EF Core migrations to the test database.
    /// </summary>
    private async Task ApplyMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new ApplicationDbContext(options);
        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Initializes Respawn for database reset.
    /// </summary>
    private async Task InitializeRespawnerAsync()
    {
        await using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
        });
    }

    /// <summary>
    /// Resets the database to a clean state.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        if (_respawner != null)
        {
            await using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);
        }
    }

    /// <inheritdoc />
    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    /// <summary>
    /// Creates a new scope for accessing scoped services.
    /// </summary>
    /// <returns>A service scope.</returns>
    public IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }
}