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
using System.Net.Http.Json;
using Tfs.Portfolio.Infrastructure.Persistence;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.Domain.Clients.Repositories;
using Tfs.Portfolio.Domain.Sectors.Repositories;
using Tfs.Portfolio.Domain.Services.Repositories;
using Tfs.Portfolio.Domain.CompanyProfile.Repositories;
using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Application.Sectors.Dtos;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Projects.Entities;
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
    /// Gets the client repository.
    /// </summary>
    public IClientRepository ClientRepository { get; private set; } = null!;

    /// <summary>
    /// Gets the sector repository.
    /// </summary>
    public ISectorRepository SectorRepository { get; private set; } = null!;

    /// <summary>
    /// Gets the service repository.
    /// </summary>
    public IServiceRepository ServiceRepository { get; private set; } = null!;

    /// <summary>
    /// Gets the company profile repository.
    /// </summary>
    public ICompanyProfileRepository CompanyProfileRepository { get; private set; } = null!;

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
        ClientRepository = _scope.ServiceProvider.GetRequiredService<IClientRepository>();
        SectorRepository = _scope.ServiceProvider.GetRequiredService<ISectorRepository>();
        ServiceRepository = _scope.ServiceProvider.GetRequiredService<IServiceRepository>();
        CompanyProfileRepository = _scope.ServiceProvider.GetRequiredService<ICompanyProfileRepository>();
        UnitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Reset database to clean state before each test
        await _factory.ResetDatabaseAsync();

        // Reset CompanyProfile singleton instance for test isolation
        CompanyProfileEntity.ResetInstance();
    }

    /// <summary>
    /// Creates a test client via API and returns its ID.
    /// </summary>
    /// <param name="name">The client name.</param>
    /// <param name="email">The client email.</param>
    /// <returns>The created client's ID.</returns>
    protected async Task<Guid> CreateTestClientAsync(string name = "Test Client", string email = "test@client.com")
    {
        var request = new CreateClientCommand(
            name,
            email,
            TfsWebUrl.Create("https://client.com/logo.png"),
            "+1234567890",
            "123 Client St");
        var response = await Client.PostAsJsonAsync("/api/v1/clients", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
        return client!.Id;
    }

    /// <summary>
    /// Creates a test sector via API and returns its ID.
    /// </summary>
    /// <param name="name">The sector name.</param>
    /// <param name="description">The sector description.</param>
    /// <returns>The created sector's ID.</returns>
    protected async Task<Guid> CreateTestSectorAsync(string name = "Technology", string description = "Test sector")
    {
        var request = new CreateSectorCommand(name, description);
        var response = await Client.PostAsJsonAsync("/api/v1/sectors", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var sector = await response.Content.ReadFromJsonAsync<SectorDto>();
        return sector!.Id;
    }

    /// <summary>
    /// Creates a test service via API and returns its ID.
    /// </summary>
    /// <param name="name">The service name.</param>
    /// <param name="description">The service description.</param>
    /// <param name="category">The service category.</param>
    /// <returns>The created service's ID.</returns>
    protected async Task<Guid> CreateTestServiceAsync(string name = "Web Development", string description = "Test service", Tfs.Portfolio.Domain.Services.Entities.ServiceCategory category = Tfs.Portfolio.Domain.Services.Entities.ServiceCategory.Development)
    {
        var request = new CreateServiceCommand(name, description, category);
        var response = await Client.PostAsJsonAsync("/api/v1/services", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var service = await response.Content.ReadFromJsonAsync<ServiceDto>();
        return service!.Id;
    }

    /// <summary>
    /// Creates a test project via API and returns its ID.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description.</param>
    /// <param name="startDate">The project start date.</param>
    /// <param name="durationMonths">The project duration in months.</param>
    /// <param name="clientId">The client ID (will create one if not provided).</param>
    /// <param name="sectorId">The sector ID (will create one if not provided).</param>
    /// <param name="technologies">The project technologies.</param>
    /// <returns>The created project's ID.</returns>
    protected async Task<Guid> CreateTestProjectAsync(
        string name = "Test Project",
        string? description = "Test Description",
        YearMonth? startDate = null,
        int durationMonths = 6,
        Guid? clientId = null,
        Guid? sectorId = null,
        IReadOnlyList<string>? technologies = null)
    {
        clientId ??= await CreateTestClientAsync();
        sectorId ??= await CreateTestSectorAsync();

        var request = new CreateProjectCommand(
            name,
            description,
            startDate ?? YearMonth.Create(6, 2024),
            durationMonths,
            clientId.Value,
            sectorId.Value,
            technologies ?? new List<string> { "C#", ".NET" });

        var response = await Client.PostAsJsonAsync("/api/v1/projects", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        return project!.Id;
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