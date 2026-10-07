// <copyright file="SectorTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain.Sectors;

using Tfs.Portfolio.Domain.Sectors.Entities;
using Tfs.Portfolio.Domain.Sectors.Exceptions;
using Tfs.Portfolio.Domain.Sectors.Events;
using Xunit;

/// <summary>
/// Unit tests for <see cref="Sector"/> aggregate.
/// </summary>
public sealed class SectorTests
{
    /// <summary>
    /// Tests that Sector can be created with valid data.
    /// </summary>
    [Fact]
    public void Create_WithValidData_RaisesSectorCreatedEvent()
    {
        // Act
        var sector = Sector.Create("FinTech", "Financial technology sector");

        // Assert
        Assert.NotEqual(Guid.Empty, sector.Id);
        Assert.Equal("FinTech", sector.Name);
        Assert.Equal("Financial technology sector", sector.Description);
        Assert.True(sector.IsActive);
        Assert.True(sector.CreatedAt <= DateTime.UtcNow);
        Assert.Null(sector.UpdatedAt);

        // Verify domain event
        var events = sector.Events;
        Assert.Single(events);
        var createdEvent = Assert.IsType<SectorCreatedEvent>(events[0]);
        Assert.Equal(sector.Id, createdEvent.SectorId);
        Assert.Equal("FinTech", createdEvent.SectorName);
    }

    /// <summary>
    /// Tests that Sector creation fails with empty name.
    /// </summary>
    [Fact]
    public void Create_WithEmptyName_ThrowsInvalidSectorStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidSectorStateException>(() => Sector.Create(string.Empty));
        Assert.Contains("Sector name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Sector creation fails with whitespace name.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceName_ThrowsInvalidSectorStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidSectorStateException>(() => Sector.Create("   "));
        Assert.Contains("Sector name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Sector creation fails with name exceeding max length.
    /// </summary>
    [Fact]
    public void Create_WithNameTooLong_ThrowsInvalidSectorStateException()
    {
        // Arrange
        var longName = new string('a', 101);

        // Act & Assert
        var exception = Assert.Throws<InvalidSectorStateException>(() => Sector.Create(longName));
        Assert.Contains("Sector name must not exceed 100 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Sector creation trims whitespace from name.
    /// </summary>
    [Fact]
    public void Create_TrimsName()
    {
        // Act
        var sector = Sector.Create("  FinTech  ");

        // Assert
        Assert.Equal("FinTech", sector.Name);
    }

    /// <summary>
    /// Tests that Sector can be created without description.
    /// </summary>
    [Fact]
    public void Create_WithoutDescription_Succeeds()
    {
        // Act
        var sector = Sector.Create("FinTech");

        // Assert
        Assert.Equal("FinTech", sector.Name);
        Assert.Null(sector.Description);
    }

    /// <summary>
    /// Tests that Sector creation fails with description exceeding max length.
    /// </summary>
    [Fact]
    public void Create_WithDescriptionTooLong_ThrowsInvalidSectorStateException()
    {
        // Arrange
        var longDescription = new string('a', 501);

        // Act & Assert
        var exception = Assert.Throws<InvalidSectorStateException>(() => Sector.Create("FinTech", longDescription));
        Assert.Contains("Sector description must not exceed 500 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Sector can be updated with valid data.
    /// </summary>
    [Fact]
    public void Update_WithValidData_UpdatesSectorAndRaisesEvent()
    {
        // Arrange
        var sector = Sector.Create("FinTech", "Financial technology sector");
        var originalCreatedAt = sector.CreatedAt;

        // Act
        sector.Update("FinTech Updated", "Updated financial technology sector");

        // Assert
        Assert.Equal("FinTech Updated", sector.Name);
        Assert.Equal("Updated financial technology sector", sector.Description);
        Assert.NotNull(sector.UpdatedAt);
        Assert.True(sector.UpdatedAt > originalCreatedAt);
        Assert.Equal(originalCreatedAt, sector.CreatedAt);

        // Verify domain event (last event should be SectorUpdatedEvent)
        var events = sector.Events;
        Assert.Equal(2, events.Count); // SectorCreatedEvent + SectorUpdatedEvent
        var updatedEvent = Assert.IsType<SectorUpdatedEvent>(events[1]);
        Assert.Equal(sector.Id, updatedEvent.SectorId);
    }

    /// <summary>
    /// Tests that Sector update fails with empty name.
    /// </summary>
    [Fact]
    public void Update_WithEmptyName_ThrowsInvalidSectorStateException()
    {
        // Arrange
        var sector = Sector.Create("FinTech");

        // Act & Assert
        var exception = Assert.Throws<InvalidSectorStateException>(() => sector.Update(string.Empty));
        Assert.Contains("Sector name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Sector update fails with whitespace name.
    /// </summary>
    [Fact]
    public void Update_WithWhitespaceName_ThrowsInvalidSectorStateException()
    {
        // Arrange
        var sector = Sector.Create("FinTech");

        // Act & Assert
        var exception = Assert.Throws<InvalidSectorStateException>(() => sector.Update("   "));
        Assert.Contains("Sector name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Sector update fails with name exceeding max length.
    /// </summary>
    [Fact]
    public void Update_WithNameTooLong_ThrowsInvalidSectorStateException()
    {
        // Arrange
        var sector = Sector.Create("FinTech");
        var longName = new string('a', 101);

        // Act & Assert
        var exception = Assert.Throws<InvalidSectorStateException>(() => sector.Update(longName));
        Assert.Contains("Sector name must not exceed 100 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Sector update trims whitespace from name.
    /// </summary>
    [Fact]
    public void Update_TrimsName()
    {
        // Arrange
        var sector = Sector.Create("FinTech");

        // Act
        sector.Update("  FinTech Updated  ");

        // Assert
        Assert.Equal("FinTech Updated", sector.Name);
    }

    /// <summary>
    /// Tests that Sector update trims whitespace from description.
    /// </summary>
    [Fact]
    public void Update_TrimsDescription()
    {
        // Arrange
        var sector = Sector.Create("FinTech", "  Old description  ");

        // Act
        sector.Update("FinTech", "  New description  ");

        // Assert
        Assert.Equal("New description", sector.Description);
    }
}