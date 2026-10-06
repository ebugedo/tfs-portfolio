// <copyright file="IntegrationTestBase.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Tfs.Portfolio.Infrastructure.Persistence;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.Domain.Common;
using Xunit;

/// <summary>
/// Base class for integration tests.
/// Provides common setup and teardown for integration tests.
/// </summary>
[SuppressMessage("Microsoft.Usage", "CA2213", Justification = "Factory is disposed by the test framework after all tests complete")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202", Justification = "Members ordered by logical grouping")]
public abstract class IntegrationTestBase : IAsyncLifetime, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private IServiceScope _scope = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestBase"/> class.
    /// </summary>
    protected IntegrationTestBase()
    {
        _factory = new CustomWebApplicationFactory();
    }

    /// <summary>
    /// Gets the HTTP client for making API requests.
    /// </summary>
    public HttpClient Client { get; private set; } = null!;

    /// <summary>
    /// Gets the application database context.
    /// </summary>
    public ApplicationDbContext DbContext { get; private set; } = null!;

    /// <summary>
    /// Gets the project repository.
    /// </summary>
    public IProjectRepository ProjectRepository { get; private set; } = null!;

    /// <summary>
    /// Gets the unit of work.
    /// </summary>
    public IUnitOfWork UnitOfWork { get; private set; } = null!;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        // Initialize the factory first (starts Testcontainers, sets up Respawn)
        await _factory.InitializeAsync();

        // Create HTTP client to trigger host creation and migrations
        Client = _factory.CreateClient();

        // Create a scope for scoped services
        _scope = _factory.Services.CreateScope();

        // Get scoped services
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ProjectRepository = _scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        UnitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Reset database to clean state before each test
        await _factory.ResetDatabaseAsync();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the resources.
    /// </summary>
    /// <param name="disposing">Whether disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _scope?.Dispose();
        }
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        Dispose();
        await _factory.DisposeAsync();
    }
}