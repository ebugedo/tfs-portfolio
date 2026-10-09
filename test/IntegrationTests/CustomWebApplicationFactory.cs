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
using Npgsql;
using Respawn;
using Respawn.Graph;
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
    [SuppressMessage("Globalization", "CA1303", Justification = "Debug logging in test infrastructure, not user-facing")]
    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private Respawner? _respawner;
        private NpgsqlDataSource? _dataSource;

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

        /// <summary>
        /// Gets or creates the Npgsql data source with dynamic JSON enabled.
        /// </summary>
        private NpgsqlDataSource GetDataSource()
        {
            if (_dataSource == null)
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
                dataSourceBuilder.EnableDynamicJson();
                _dataSource = dataSourceBuilder.Build();
            }
            return _dataSource;
        }

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
                    var dataSource = GetDataSource();
                    options.UseNpgsql(dataSource)
                        .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)
                            .Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ManyServiceProvidersCreatedWarning));
                });
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
            var dataSource = GetDataSource();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(dataSource)
                .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)
                    .Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ManyServiceProvidersCreatedWarning))
                .EnableSensitiveDataLogging()
                .Options;

            // Retry logic to handle database not being ready yet
            const int maxRetries = 60;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await using var context = new ApplicationDbContext(options);

                    // Check if we can connect
                    var canConnect = await context.Database.CanConnectAsync();
                    Console.WriteLine($"[ApplyMigrationsAsync] Attempt {i + 1}/{maxRetries}: CanConnect = {canConnect}");

                    if (!canConnect)
                    {
                        await Task.Delay(2000);
                        continue;
                    }

                    // Use EnsureCreated instead of Migrate because migrations have compilation issues
                    // (value object mappings generate invalid C# in migration files)
                    Console.WriteLine("[ApplyMigrationsAsync] Ensuring database is created from model...");
                    await context.Database.EnsureCreatedAsync();
                    Console.WriteLine("[ApplyMigrationsAsync] Database created successfully");

                    // Verify tables exist
                    var tablesAfter = await context.Database.SqlQueryRaw<string>("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'").ToListAsync();
                    Console.WriteLine($"[ApplyMigrationsAsync] Tables in database: {string.Join(", ", tablesAfter)}");
                    return; // Success
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine($"[ApplyMigrationsAsync] NpgsqlException on attempt {i + 1}: {ex.Message}");
                    await Task.Delay(2000);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    Console.WriteLine($"[ApplyMigrationsAsync] DbUpdateException on attempt {i + 1}: {ex.Message}");
                    await Task.Delay(2000);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"[ApplyMigrationsAsync] InvalidOperationException on attempt {i + 1}: {ex.Message}");
                    await Task.Delay(2000);
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"[ApplyMigrationsAsync] TimeoutException on attempt {i + 1}: {ex.Message}");
                    await Task.Delay(2000);
                }
            }

            throw new InvalidOperationException("Failed to create database after retries");
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
            if (_dataSource != null)
            {
                await _dataSource.DisposeAsync();
            }
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