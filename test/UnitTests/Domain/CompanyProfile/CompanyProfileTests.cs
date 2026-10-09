// <copyright file="CompanyProfileTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain.CompanyProfile;

using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Domain.CompanyProfile.Exceptions;
using Tfs.Portfolio.Domain.CompanyProfile.Events;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Xunit;

/// <summary>
/// Unit tests for <see cref="CompanyProfileEntity"/> aggregate.
/// </summary>
public sealed class CompanyProfileTests
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileTests"/> class.
    /// Resets the singleton instance before each test.
    /// </summary>
    public CompanyProfileTests()
    {
        CompanyProfileEntity.ResetInstance();
    }

    /// <summary>
    /// Tests that CompanyProfile can be created with valid data.
    /// </summary>
    [Fact]
    public void Create_WithValidData_RaisesCompanyProfileCreatedEvent()
    {
        // Act
        var profile = CompanyProfileEntity.Create(
            "Tfs Portfolio",
            "contact@tfsportfolio.com",
            "+1234567890",
            TfsWebUrl.Create("https://tfsportfolio.com"),
            "123 Main St",
            "A software consulting company",
            TfsWebUrl.Create("https://tfsportfolio.com/logo.png"));

        // Assert
        Assert.NotEqual(Guid.Empty, profile.Id);
        Assert.Equal("Tfs Portfolio", profile.CompanyName);
        Assert.Equal("contact@tfsportfolio.com", profile.ContactEmail);
        Assert.Equal("+1234567890", profile.ContactPhone);
        Assert.Equal("https://tfsportfolio.com", profile.TfsWebUrl?.Value);
        Assert.Equal("123 Main St", profile.Address);
        Assert.Equal("A software consulting company", profile.Description);
        Assert.Equal("https://tfsportfolio.com/logo.png", profile.LogoUrl?.Value);
        Assert.True(profile.CreatedAt <= DateTime.UtcNow);
        Assert.Null(profile.UpdatedAt);

        // Verify domain event
        var events = profile.Events;
        Assert.Single(events);
        var createdEvent = Assert.IsType<CompanyProfileCreatedEvent>(events[0]);
        Assert.Equal(profile.Id, createdEvent.ProfileId);
        Assert.Equal("Tfs Portfolio", createdEvent.CompanyName);
    }

    /// <summary>
    /// Tests that CompanyProfile can be created with only required fields.
    /// </summary>
    [Fact]
    public void Create_WithOnlyRequiredFields_Succeeds()
    {
        // Act
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Assert
        Assert.Equal("Tfs Portfolio", profile.CompanyName);
        Assert.Equal("contact@tfsportfolio.com", profile.ContactEmail);
        Assert.Null(profile.ContactPhone);
        Assert.Null(profile.TfsWebUrl);
        Assert.Null(profile.Address);
        Assert.Null(profile.Description);
        Assert.Null(profile.LogoUrl);
    }

    /// <summary>
    /// Tests that CompanyProfile creation fails when instance already exists (singleton enforcement).
    /// </summary>
    [Fact]
    public void Create_WhenInstanceAlreadyExists_ThrowsCompanyProfileAlreadyExistsException()
    {
        // Arrange
        CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Act & Assert
        var exception = Assert.Throws<CompanyProfileAlreadyExistsException>(
            () => CompanyProfileEntity.Create("Another Company", "another@company.com"));

        Assert.Contains("Company profile already exists", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile creation fails with empty company name.
    /// </summary>
    [Fact]
    public void Create_WithEmptyCompanyName_ThrowsInvalidCompanyProfileStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidCompanyProfileStateException>(
            () => CompanyProfileEntity.Create(string.Empty, "contact@tfsportfolio.com"));

        Assert.Contains("Company name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile creation fails with whitespace company name.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceCompanyName_ThrowsInvalidCompanyProfileStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidCompanyProfileStateException>(
            () => CompanyProfileEntity.Create("   ", "contact@tfsportfolio.com"));

        Assert.Contains("Company name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile creation fails with company name exceeding max length.
    /// </summary>
    [Fact]
    public void Create_WithCompanyNameTooLong_ThrowsInvalidCompanyProfileStateException()
    {
        // Arrange
        var longName = new string('a', 201);

        // Act & Assert
        var exception = Assert.Throws<InvalidCompanyProfileStateException>(
            () => CompanyProfileEntity.Create(longName, "contact@tfsportfolio.com"));

        Assert.Contains("Company name must not exceed 200 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile creation fails with invalid contact email.
    /// </summary>
    [Fact]
    public void Create_WithInvalidContactEmail_ThrowsInvalidCompanyProfileStateException()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidCompanyProfileStateException>(
            () => CompanyProfileEntity.Create("Tfs Portfolio", "invalid-email"));

        Assert.Contains("Invalid contact email format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile creation fails with whitespace contact email.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceContactEmail_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => CompanyProfileEntity.Create("Tfs Portfolio", "   "));

        Assert.Contains("cannot be an empty string", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile creation fails with invalid web URL.
    /// </summary>
    [Fact]
    public void Create_WithInvalidTfsWebUrl_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com", webUrl: TfsWebUrl.Create("not-a-url")));

        Assert.Contains("Invalid URL format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile creation fails with invalid logo URL.
    /// </summary>
    [Fact]
    public void Create_WithInvalidLogoUrl_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com", logoUrl: TfsWebUrl.Create("not-a-url")));

        Assert.Contains("Invalid URL format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile creation trims whitespace from company name.
    /// </summary>
    [Fact]
    public void Create_TrimsCompanyName()
    {
        // Act
        var profile = CompanyProfileEntity.Create("  Tfs Portfolio  ", "contact@tfsportfolio.com");

        // Assert
        Assert.Equal("Tfs Portfolio", profile.CompanyName);
    }

    /// <summary>
    /// Tests that CompanyProfile can be updated with valid data.
    /// </summary>
    [Fact]
    public void Update_WithValidData_UpdatesProfileAndRaisesEvent()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");
        var originalCreatedAt = profile.CreatedAt;

        // Act
        profile.Update(
            "Tfs Portfolio Updated",
            "updated@tfsportfolio.com",
            "+9876543210",
            TfsWebUrl.Create("https://updated.com"),
            "456 Updated Ave",
            "Updated description",
            TfsWebUrl.Create("https://updated.com/new-logo.png"));

        // Assert
        Assert.Equal("Tfs Portfolio Updated", profile.CompanyName);
        Assert.Equal("updated@tfsportfolio.com", profile.ContactEmail);
        Assert.Equal("+9876543210", profile.ContactPhone);
        Assert.Equal("https://updated.com", profile.TfsWebUrl?.Value);
        Assert.Equal("456 Updated Ave", profile.Address);
        Assert.Equal("Updated description", profile.Description);
        Assert.Equal("https://updated.com/new-logo.png", profile.LogoUrl?.Value);
        Assert.NotNull(profile.UpdatedAt);
        Assert.True(profile.UpdatedAt > originalCreatedAt);
        Assert.Equal(originalCreatedAt, profile.CreatedAt);

        // Verify domain event (last event should be CompanyProfileUpdatedEvent)
        var events = profile.Events;
        Assert.Equal(2, events.Count); // CompanyProfileCreatedEvent + CompanyProfileUpdatedEvent
        var updatedEvent = Assert.IsType<CompanyProfileUpdatedEvent>(events[1]);
        Assert.Equal(profile.Id, updatedEvent.ProfileId);
    }

    /// <summary>
    /// Tests that CompanyProfile update fails with empty company name.
    /// </summary>
    [Fact]
    public void Update_WithEmptyCompanyName_ThrowsInvalidCompanyProfileStateException()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Act & Assert
        var exception = Assert.Throws<InvalidCompanyProfileStateException>(
            () => profile.Update(string.Empty, "contact@tfsportfolio.com"));

        Assert.Contains("Company name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile update fails with whitespace company name.
    /// </summary>
    [Fact]
    public void Update_WithWhitespaceCompanyName_ThrowsInvalidCompanyProfileStateException()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Act & Assert
        var exception = Assert.Throws<InvalidCompanyProfileStateException>(
            () => profile.Update("   ", "contact@tfsportfolio.com"));

        Assert.Contains("Company name cannot be empty", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile update fails with company name exceeding max length.
    /// </summary>
    [Fact]
    public void Update_WithCompanyNameTooLong_ThrowsInvalidCompanyProfileStateException()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");
        var longName = new string('a', 201);

        // Act & Assert
        var exception = Assert.Throws<InvalidCompanyProfileStateException>(
            () => profile.Update(longName, "contact@tfsportfolio.com"));

        Assert.Contains("Company name must not exceed 200 characters", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile update fails with invalid contact email.
    /// </summary>
    [Fact]
    public void Update_WithInvalidContactEmail_ThrowsInvalidCompanyProfileStateException()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Act & Assert
        var exception = Assert.Throws<InvalidCompanyProfileStateException>(
            () => profile.Update("Tfs Portfolio", "invalid-email"));

        Assert.Contains("Invalid contact email format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile update fails with invalid web URL.
    /// </summary>
    [Fact]
    public void Update_WithInvalidTfsWebUrl_ThrowsArgumentException()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => profile.Update("Tfs Portfolio", "contact@tfsportfolio.com", webUrl: TfsWebUrl.Create("not-a-url")));

        Assert.Contains("Invalid URL format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile update fails with invalid logo URL.
    /// </summary>
    [Fact]
    public void Update_WithInvalidLogoUrl_ThrowsArgumentException()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => profile.Update("Tfs Portfolio", "contact@tfsportfolio.com", logoUrl: TfsWebUrl.Create("not-a-url")));

        Assert.Contains("Invalid URL format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that CompanyProfile update trims whitespace from fields.
    /// </summary>
    [Fact]
    public void Update_TrimsWhitespace()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Act
        profile.Update(
            "  Tfs Portfolio Updated  ",
            "  updated@tfsportfolio.com  ",
            "  +9876543210  ",
            TfsWebUrl.Create("https://updated.com"),
            "  456 Updated Ave  ",
            "  Updated description  ",
            TfsWebUrl.Create("https://updated.com/new-logo.png"));

        // Assert
        Assert.Equal("Tfs Portfolio Updated", profile.CompanyName);
        Assert.Equal("updated@tfsportfolio.com", profile.ContactEmail);
        Assert.Equal("+9876543210", profile.ContactPhone);
        Assert.Equal("456 Updated Ave", profile.Address);
        Assert.Equal("Updated description", profile.Description);
    }

    /// <summary>
    /// Tests that singleton instance is accessible via static property.
    /// </summary>
    [Fact]
    public void Instance_AfterCreation_ReturnsSingleton()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create("Tfs Portfolio", "contact@tfsportfolio.com");

        // Act
        var instance = CompanyProfileEntity.Instance;

        // Assert
        Assert.NotNull(instance);
        Assert.Equal(profile.Id, instance!.Id);
    }

    /// <summary>
    /// Tests that singleton instance is null before creation.
    /// </summary>
    [Fact]
    public void Instance_BeforeCreation_ReturnsNull()
    {
        // Act
        var instance = CompanyProfileEntity.Instance;

        // Assert
        Assert.Null(instance);
    }
}