// <copyright file="CompanyProfileRepositoryTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Persistence;

using Microsoft.EntityFrameworkCore;
using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Domain.CompanyProfile.Exceptions;
using Tfs.Portfolio.Domain.CompanyProfile.Repositories;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.IntegrationTests;
using Xunit;
using FluentAssertions;

/// <summary>
/// Integration tests for the CompanyProfileRepository.
/// </summary>
public sealed class CompanyProfileRepositoryTests : IntegrationTestBase
{
    /// <summary>
    /// Tests that AddAsync persists a company profile to the database.
    /// </summary>
    [Fact]
    public async Task AddAsync_PersistsCompanyProfileToDatabase()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create(
            companyName: "Tfs.Portfolio",
            contactEmail: "contact@tfsportfolio.com",
            webUrl: TfsWebUrl.Create("https://tfsportfolio.com"),
            address: "123 Portfolio St",
            description: "A software consulting company",
            logoUrl: TfsWebUrl.Create("https://tfsportfolio.com/logo.png"));

        // Act
        await CompanyProfileRepository.AddAsync(profile);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the profile was persisted by retrieving it
        var retrieved = await CompanyProfileRepository.GetAsync();
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(profile.Id);
        retrieved.CompanyName.Should().Be("Tfs.Portfolio");
        retrieved.ContactEmail.Should().Be("contact@tfsportfolio.com");
        var tfsWebUrl = retrieved.TfsWebUrl!.Value.Value;
        tfsWebUrl.Should().Be("https://tfsportfolio.com");
        retrieved.Address.Should().Be("123 Portfolio St");
        retrieved.Description.Should().Be("A software consulting company");
        var logoUrl = retrieved.LogoUrl!.Value.Value;
        logoUrl.Should().Be("https://tfsportfolio.com/logo.png");
        retrieved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that GetAsync returns the company profile when it exists.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenExists_ReturnsCompanyProfile()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create(
            companyName: "Test Company",
            contactEmail: "test@company.com");
        await CompanyProfileRepository.AddAsync(profile);
        await UnitOfWork.SaveChangesAsync();

        // Act
        var retrieved = await CompanyProfileRepository.GetAsync();

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(profile.Id);
        retrieved.CompanyName.Should().Be("Test Company");
        retrieved.ContactEmail.Should().Be("test@company.com");
    }

    /// <summary>
    /// Tests that GetAsync returns null when no company profile exists.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenNotExists_ReturnsNull()
    {
        // Act
        var result = await CompanyProfileRepository.GetAsync();

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Tests that AddAsync throws when a company profile already exists (singleton constraint).
    /// </summary>
    [Fact]
    public async Task AddAsync_WhenAlreadyExists_ThrowsCompanyProfileAlreadyExistsException()
    {
        // Arrange
        var profile1 = CompanyProfileEntity.Create(
            companyName: "First Company",
            contactEmail: "first@company.com");
        await CompanyProfileRepository.AddAsync(profile1);
        await UnitOfWork.SaveChangesAsync();

        // Act & Assert - Create throws synchronously due to singleton constraint
        var exception = Assert.Throws<CompanyProfileAlreadyExistsException>(
            () => CompanyProfileEntity.Create(
                companyName: "Second Company",
                contactEmail: "second@company.com"));
        exception.Message.Should().Be("Company profile already exists");
    }

    /// <summary>
    /// Tests that Update updates an existing company profile.
    /// </summary>
    [Fact]
    public async Task Update_UpdatesExistingCompanyProfile()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create(
            companyName: "Original Company",
            contactEmail: "original@company.com");
        await CompanyProfileRepository.AddAsync(profile);
        await UnitOfWork.SaveChangesAsync();

        // Detach to simulate new request scope
        DbContext.Entry(profile).State = EntityState.Detached;

        // Act - Get tracked entity, modify it, save
        var trackedProfile = await CompanyProfileRepository.GetAsync();
        trackedProfile.Should().NotBeNull();
        trackedProfile!.Update(
            companyName: "Updated Company",
            contactEmail: "updated@company.com",
            webUrl: TfsWebUrl.Create("https://updated.com"),
            address: "789 Updated Blvd",
            description: "Updated description",
            logoUrl: TfsWebUrl.Create("https://updated.com/logo.png"));
        await UnitOfWork.SaveChangesAsync();

        // Assert - read from clean context to verify persisted state
        var retrieved = await DbContext.CompanyProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == profile.Id);
        retrieved.Should().NotBeNull();
        retrieved!.CompanyName.Should().Be("Updated Company");
        retrieved.ContactEmail.Should().Be("updated@company.com");
        var tfsWebUrl = retrieved.TfsWebUrl!.Value.Value;
        tfsWebUrl.Should().Be("https://updated.com");
        retrieved.Address.Should().Be("789 Updated Blvd");
        retrieved.Description.Should().Be("Updated description");
        var logoUrl = retrieved.LogoUrl!.Value.Value;
        logoUrl.Should().Be("https://updated.com/logo.png");
        retrieved.UpdatedAt.Should().NotBeNull();
        retrieved.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests that Delete removes a company profile.
    /// </summary>
    [Fact]
    public async Task Delete_RemovesCompanyProfile()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create(
            companyName: "Delete Test",
            contactEmail: "delete@company.com");
        await CompanyProfileRepository.AddAsync(profile);
        await UnitOfWork.SaveChangesAsync();

        // Act
        CompanyProfileRepository.Delete(profile);
        await UnitOfWork.SaveChangesAsync();

        // Assert
        var retrieved = await CompanyProfileRepository.GetAsync();
        retrieved.Should().BeNull();
    }

    /// <summary>
    /// Tests that UnitOfWork commits the transaction.
    /// </summary>
    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_CommitsTransaction()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create(
            companyName: "Transaction Test",
            contactEmail: "transaction@company.com");
        await CompanyProfileRepository.AddAsync(profile);

        // Act
        var rowsAffected = await UnitOfWork.SaveChangesAsync();

        // Assert
        rowsAffected.Should().Be(1);

        // Verify the profile was actually persisted
        var retrieved = await CompanyProfileRepository.GetAsync();
        retrieved.Should().NotBeNull();
        retrieved!.CompanyName.Should().Be("Transaction Test");
    }

    /// <summary>
    /// Tests that only one company profile can exist (singleton at database level).
    /// </summary>
    [Fact]
    public async Task SingletonConstraint_OnlyOneProfileAllowed()
    {
        // Arrange
        var profile1 = CompanyProfileEntity.Create(
            companyName: "Company 1",
            contactEmail: "company1@test.com");
        await CompanyProfileRepository.AddAsync(profile1);
        await UnitOfWork.SaveChangesAsync();

        // Act & Assert - verify only one exists in database
        var allProfiles = await DbContext.CompanyProfiles
            .AsNoTracking()
            .ToListAsync();
        allProfiles.Should().HaveCount(1);
    }

    /// <summary>
    /// Tests that CompanyProfile with null TfsWebUrl and LogoUrl persists and loads correctly.
    /// </summary>
    [Fact]
    public async Task AddAsync_WithNullUrls_PersistsAndLoadsCorrectly()
    {
        // Arrange
        var profile = CompanyProfileEntity.Create(
            companyName: "Null URLs Test",
            contactEmail: "nullurls@company.com",
            address: "123 Null St",
            description: "Description");

        // Act
        await CompanyProfileRepository.AddAsync(profile);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the profile was persisted and URLs are null
        var retrieved = await CompanyProfileRepository.GetAsync();
        retrieved.Should().NotBeNull();
        retrieved!.TfsWebUrl.Should().BeNull();
        retrieved.LogoUrl.Should().BeNull();
    }

    /// <summary>
    /// Tests that CompanyProfile with non-null TfsWebUrl and LogoUrl persists and loads correctly.
    /// </summary>
    [Fact]
    public async Task AddAsync_WithNonNullUrls_PersistsAndLoadsCorrectly()
    {
        // Arrange
        var tfsWebUrl = "https://company.com";
        var logoUrl = "https://company.com/logo.png";
        var profile = CompanyProfileEntity.Create(
            companyName: "Non-Null URLs Test",
            contactEmail: "nonnull@company.com",
            webUrl: TfsWebUrl.Create(tfsWebUrl),
            address: "123 NonNull St",
            description: "Description",
            logoUrl: TfsWebUrl.Create(logoUrl));

        // Act
        await CompanyProfileRepository.AddAsync(profile);
        await UnitOfWork.SaveChangesAsync();

        // Assert - verify the profile was persisted and URLs are correct
        var retrieved = await CompanyProfileRepository.GetAsync();
        retrieved.Should().NotBeNull();
        retrieved!.TfsWebUrl.Should().NotBeNull();
        var tfsWebUrlValue = retrieved.TfsWebUrl!.Value.Value;
        tfsWebUrlValue.Should().Be(tfsWebUrl);
        var logoUrlValue = retrieved.LogoUrl!.Value.Value;
        logoUrlValue.Should().Be(logoUrl);
    }
}