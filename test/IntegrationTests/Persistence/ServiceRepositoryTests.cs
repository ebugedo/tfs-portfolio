// <copyright file="ServiceRepositoryTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Persistence;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.Domain.Services.Repositories;
using Tfs.Portfolio.IntegrationTests;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the ServiceRepository.
/// </summary>
public sealed class ServiceRepositoryTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that AddAsync persists a service to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_PersistsServiceToDatabase()
    {
        // Arrange
        var service = Service.Create("Web Development", "Custom web application development", ServiceCategory.Development);

        // Act
        await ServiceRepository.AddAsync(service);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the service was persisted by retrieving it
        var retrieved = await ServiceRepository.GetByIdAsync(service.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(service.Id);
        retrieved.Name.Should().Be("Web Development");
        retrieved.Description.Should().Be("Custom web application development");
        retrieved.Category.Should().Be(ServiceCategory.Development);
        retrieved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that GetByIdAsync returns the correct entity.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectEntity()
    {
        // Arrange
        var service = Service.Create("GetById Service", "Service for get by ID test", ServiceCategory.Design);
        await ServiceRepository.AddAsync(service);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var retrieved = await ServiceRepository.GetByIdAsync(service.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(service.Id);
        retrieved.Name.Should().Be("GetById Service");
        retrieved.Description.Should().Be("Service for get by ID test");
        retrieved.Category.Should().Be(ServiceCategory.Design);
    }

    /// <summary>
    /// Tests that GetByIdAsync returns null for non-existent ID.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var result = await ServiceRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that GetAllAsync returns all services.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsAllServices()
    {
        // Arrange
        var service1 = Service.Create("Service 1", "First service", ServiceCategory.Development);
        var service2 = Service.Create("Service 2", "Second service", ServiceCategory.Consulting);
        var service3 = Service.Create("Service 3", "Third service", ServiceCategory.Design);

        await ServiceRepository.AddAsync(service1);
        await ServiceRepository.AddAsync(service2);
        await ServiceRepository.AddAsync(service3);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var services = await ServiceRepository.GetAllAsync();

        // Assert
        services.Should().NotBeNull();
        services.Should().HaveCount(3);
        services.Should().Contain(s => s.Name == "Service 1");
        services.Should().Contain(s => s.Name == "Service 2");
        services.Should().Contain(s => s.Name == "Service 3");
    }

    /// <summary>
    /// Tests that GetActiveAsync returns all active services.
    /// </summary>
    [Fact]
    public async Task GetActiveAsync_ReturnsActiveServices()
    {
        // Arrange - by default services are active on creation
        var service1 = Service.Create("Active Service 1", "First active service", ServiceCategory.Development);
        var service2 = Service.Create("Active Service 2", "Second active service", ServiceCategory.Consulting);

        await ServiceRepository.AddAsync(service1);
        await ServiceRepository.AddAsync(service2);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var activeServices = await ServiceRepository.GetActiveAsync();

        // Assert
        activeServices.Should().NotBeNull();
        activeServices.Should().HaveCount(2);
        activeServices.Should().Contain(s => s.Name == "Active Service 1");
        activeServices.Should().Contain(s => s.Name == "Active Service 2");
    }

    /// <summary>
    /// Tests that GetByCategoryAsync returns services filtered by category.
    /// </summary>
    [Fact]
    public async Task GetByCategoryAsync_ReturnsFilteredServices()
    {
        // Arrange
        var devService = Service.Create("Dev Service", "Development service", ServiceCategory.Development);
        var consultingService = Service.Create("Consulting Service", "Consulting service", ServiceCategory.Consulting);
        var designService = Service.Create("Design Service", "Design service", ServiceCategory.Design);

        await ServiceRepository.AddAsync(devService);
        await ServiceRepository.AddAsync(consultingService);
        await ServiceRepository.AddAsync(designService);
        await UnitOfWork.SaveChangesAsync();

        // Act - get services by Development category
        var devServices = await ServiceRepository.GetByCategoryAsync(ServiceCategory.Development);

        // Assert
        devServices.Should().NotBeNull();
        devServices.Should().HaveCount(1);
        devServices.Should().Contain(s => s.Name == "Dev Service");
        devServices.Should().NotContain(s => s.Name == "Consulting Service");
        devServices.Should().NotContain(s => s.Name == "Design Service");

        // Act - get services by Consulting category
        var consultingServices = await ServiceRepository.GetByCategoryAsync(ServiceCategory.Consulting);

        // Assert
        consultingServices.Should().NotBeNull();
        consultingServices.Should().HaveCount(1);
        consultingServices.Should().Contain(s => s.Name == "Consulting Service");
    }

    /// <summary>
    /// Tests that Update updates an existing service.
    /// </summary>
    [Fact]
    public async Task Update_UpdatesExistingService()
    {
        // Arrange
        var service = Service.Create("Original Name", "Original description", ServiceCategory.DevOps);
        await ServiceRepository.AddAsync(service);
        await UnitOfWork.SaveChangesAsync();

        // Detach to simulate new request scope
        DbContext.Entry(service).State = EntityState.Detached;

        // Act - Get tracked entity, modify it, save
        var trackedService = await ServiceRepository.GetByIdAsync(service.Id);
        trackedService.Should().NotBeNull();
        trackedService!.Update("Updated Name", "Updated description", ServiceCategory.Training);
        await UnitOfWork.SaveChangesAsync();

        // Assert - read from clean context to verify persisted state
        var retrieved = await DbContext.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == service.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Updated Name");
        retrieved.Description.Should().Be("Updated description");
        retrieved.Category.Should().Be(ServiceCategory.Training);
    }

    /// <summary>
    /// Tests that Delete removes a service.
    /// </summary>
    [Fact]
    public async Task Delete_RemovesService()
    {
        // Arrange
        var service = Service.Create("Delete Test", "Service to be deleted", ServiceCategory.Development);
        await ServiceRepository.AddAsync(service);
        await UnitOfWork.SaveChangesAsync();

        // Act
        ServiceRepository.Delete(service);
        await UnitOfWork.SaveChangesAsync();

        // Assert
        var retrieved = await ServiceRepository.GetByIdAsync(service.Id);
        retrieved.Should().BeNull();
    }

    /// <summary>
    /// Tests that UnitOfWork commits the transaction.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_CommitsTransaction()
    {
        // Arrange
        var service = Service.Create("Transaction Test", "Test description", ServiceCategory.Development);
        await ServiceRepository.AddAsync(service);

        // Act
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert
        rowsAffected.Should().Be(1);

        // Verify the service was actually persisted
        var retrieved = await ServiceRepository.GetByIdAsync(service.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Transaction Test");
    }

    /// <summary>
    /// Tests that multiple operations in a single transaction are atomic.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_MultipleOperations_AreAtomic()
    {
        // Arrange
        var service1 = Service.Create("Atomic Service 1", "Description 1", ServiceCategory.Development);
        var service2 = Service.Create("Atomic Service 2", "Description 2", ServiceCategory.Consulting);

        // Act - add both but only save once
        await ServiceRepository.AddAsync(service1);
        await ServiceRepository.AddAsync(service2);
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert - both should be saved in a single transaction
        rowsAffected.Should().Be(2);

        var allServices = await ServiceRepository.GetAllAsync();
        allServices.Should().HaveCount(2);
    }
}