// <copyright file="ServiceTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain.Services;

using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.Domain.Services.Exceptions;
using Tfs.Portfolio.Domain.Services.Events;
using Xunit;

/// <summary>
/// Unit tests for <see cref="Service"/> aggregate.
/// </summary>
public sealed class ServiceTests
{
    /// <summary>
    /// Tests that Service can be created with valid data and description.
    /// </summary>
    [Fact]
    public void Create_WithValidData_RaisesServiceCreatedEvent()
    {
        // Act
        var service = Service.Create("Web Development", "Building web applications", ServiceCategory.Development);

        // Assert
        Assert.NotEqual(Guid.Empty, service.Id);
        Assert.Equal("Web Development", service.Name);
        Assert.Equal("Building web applications", service.Description);
        Assert.Equal(ServiceCategory.Development, service.Category);
        Assert.True(service.IsActive);
        Assert.True(service.CreatedAt <= DateTime.UtcNow);
        Assert.Null(service.UpdatedAt);

        // Verify domain event
        var events = service.Events;
        Assert.Single(events);
        var createdEvent = Assert.IsType<ServiceCreatedEvent>(events[0]);
        Assert.Equal(service.Id, createdEvent.ServiceId);
        Assert.Equal("Web Development", createdEvent.ServiceName);
    }

    /// <summary>
    /// Tests that Service can be created without description.
    /// </summary>
    [Fact]
    public void Create_WithoutDescription_Succeeds()
    {
        // Act
        var service = Service.Create("Web Development", null, ServiceCategory.Development);

        // Assert
        Assert.Equal("Web Development", service.Name);
        Assert.Null(service.Description);
    }

    /// <summary>
    /// Tests that Service creation fails with empty name.
    /// </summary>
    [Fact]
    public void Create_WithEmptyName_ThrowsInvalidServiceStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidServiceStateException>(() => Service.Create(string.Empty, null, ServiceCategory.Development));
        Assert.Contains("Service name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Service creation fails with whitespace name.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceName_ThrowsInvalidServiceStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidServiceStateException>(() => Service.Create("   ", null, ServiceCategory.Development));
        Assert.Contains("Service name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Service creation fails with name exceeding max length.
    /// </summary>
    [Fact]
    public void Create_WithNameTooLong_ThrowsInvalidServiceStateException()
    {
        // Arrange
        var longName = new string('a', 151);

        // Act & Assert
        var exception = Assert.Throws<InvalidServiceStateException>(() => Service.Create(longName, null, ServiceCategory.Development));
        Assert.Contains("Service name must not exceed 150 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Service creation fails with description exceeding max length.
    /// </summary>
    [Fact]
    public void Create_WithDescriptionTooLong_ThrowsInvalidServiceStateException()
    {
        // Arrange
        var longDescription = new string('a', 1001);

        // Act & Assert
        var exception = Assert.Throws<InvalidServiceStateException>(() => Service.Create("Web Development", longDescription, ServiceCategory.Development));
        Assert.Contains("Service description must not exceed 1000 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Service creation trims whitespace from name.
    /// </summary>
    [Fact]
    public void Create_TrimsName()
    {
        // Act
        var service = Service.Create("  Web Development  ", null, ServiceCategory.Development);

        // Assert
        Assert.Equal("Web Development", service.Name);
    }

    /// <summary>
    /// Tests that Service can be created with different categories.
    /// </summary>
    /// <param name="category">The service category to test.</param>
    [Theory]
    [InlineData(ServiceCategory.Development)]
    [InlineData(ServiceCategory.Consulting)]
    [InlineData(ServiceCategory.Design)]
    [InlineData(ServiceCategory.DevOps)]
    [InlineData(ServiceCategory.Training)]
    public void Create_WithDifferentCategories_Succeeds(ServiceCategory category)
    {
        // Act
        var service = Service.Create("Test Service", null, category);

        // Assert
        Assert.Equal(category, service.Category);
    }

    /// <summary>
    /// Tests that Service can be updated with valid data.
    /// </summary>
    [Fact]
    public void Update_WithValidData_UpdatesServiceAndRaisesEvent()
    {
        // Arrange
        var service = Service.Create("Web Development", null, ServiceCategory.Development);
        var originalCreatedAt = service.CreatedAt;

        // Act
        service.Update("Web Development Updated", "Updated web development service", ServiceCategory.Consulting);

        // Assert
        Assert.Equal("Web Development Updated", service.Name);
        Assert.Equal("Updated web development service", service.Description);
        Assert.Equal(ServiceCategory.Consulting, service.Category);
        Assert.NotNull(service.UpdatedAt);
        Assert.True(service.UpdatedAt > originalCreatedAt);

        // Verify domain event (last event should be ServiceUpdatedEvent)
        var events = service.Events;
        Assert.Equal(2, events.Count); // ServiceCreatedEvent + ServiceUpdatedEvent
        var updatedEvent = Assert.IsType<ServiceUpdatedEvent>(events[1]);
        Assert.Equal(service.Id, updatedEvent.ServiceId);
    }

    /// <summary>
    /// Tests that Service update fails with empty name.
    /// </summary>
    [Fact]
    public void Update_WithEmptyName_ThrowsInvalidServiceStateException()
    {
        // Arrange
        var service = Service.Create("Web Development", null, ServiceCategory.Development);

        // Act & Assert
        var exception = Assert.Throws<InvalidServiceStateException>(() => service.Update(string.Empty, null, ServiceCategory.Development));
        Assert.Contains("Service name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Service update fails with whitespace name.
    /// </summary>
    [Fact]
    public void Update_WithWhitespaceName_ThrowsInvalidServiceStateException()
    {
        // Arrange
        var service = Service.Create("Web Development", null, ServiceCategory.Development);

        // Act & Assert
        var exception = Assert.Throws<InvalidServiceStateException>(() => service.Update("   ", null, ServiceCategory.Development));
        Assert.Contains("Service name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Service update fails with name exceeding max length.
    /// </summary>
    [Fact]
    public void Update_WithNameTooLong_ThrowsInvalidServiceStateException()
    {
        // Arrange
        var service = Service.Create("Web Development", null, ServiceCategory.Development);
        var longName = new string('a', 151);

        // Act & Assert
        var exception = Assert.Throws<InvalidServiceStateException>(() => service.Update(longName, null, ServiceCategory.Development));
        Assert.Contains("Service name must not exceed 150 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Service update fails with description exceeding max length.
    /// </summary>
    [Fact]
    public void Update_WithDescriptionTooLong_ThrowsInvalidServiceStateException()
    {
        // Arrange
        var service = Service.Create("Web Development", null, ServiceCategory.Development);
        var longDescription = new string('a', 1001);

        // Act & Assert
        var exception = Assert.Throws<InvalidServiceStateException>(() => service.Update("Web Development", longDescription, ServiceCategory.Development));
        Assert.Contains("Service description must not exceed 1000 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Service update trims whitespace from name and description.
    /// </summary>
    [Fact]
    public void Update_TrimsNameAndDescription()
    {
        // Arrange
        var service = Service.Create("Web Development", null, ServiceCategory.Development);

        // Act
        service.Update("  Web Development Updated  ", "  Updated description  ", ServiceCategory.DevOps);

        // Assert
        Assert.Equal("Web Development Updated", service.Name);
        Assert.Equal("Updated description", service.Description);
        Assert.Equal(ServiceCategory.DevOps, service.Category);
    }
}