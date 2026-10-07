// <copyright file="ContactInfoTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain.ValueObjects;

using Tfs.Portfolio.Domain.Common.ValueObjects;
using Xunit;

/// <summary>
/// Unit tests for <see cref="ContactInfo"/> value object.
/// </summary>
public sealed class ContactInfoTests
{
    /// <summary>
    /// Tests that ContactInfo can be created with valid email.
    /// </summary>
    [Fact]
    public void Create_WithValidEmail_ReturnsContactInfo()
    {
        // Act
        var contactInfo = ContactInfo.Create("contact@company.com", "+1234567890", "123 Street, City");

        // Assert
        Assert.Equal("contact@company.com", contactInfo.Email);
        Assert.Equal("+1234567890", contactInfo.Phone);
        Assert.Equal("123 Street, City", contactInfo.Address);
    }

    /// <summary>
    /// Tests that ContactInfo creation fails with invalid email format.
    /// </summary>
    [Fact]
    public void Create_WithInvalidEmail_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ContactInfo.Create("invalid-email", "phone", "address"));
        Assert.Contains("Invalid email format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that ContactInfo creation fails with null email.
    /// </summary>
    [Fact]
    public void Create_WithNullEmail_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ContactInfo.Create(null!, "phone", "address"));
        Assert.Contains("Invalid email format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that ContactInfo creation fails with empty email.
    /// </summary>
    [Fact]
    public void Create_WithEmptyEmail_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ContactInfo.Create(string.Empty, "phone", "address"));
        Assert.Contains("Invalid email format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that ContactInfo handles optional phone and address.
    /// </summary>
    [Fact]
    public void Create_WithOnlyEmail_ReturnsContactInfo()
    {
        // Act
        var contactInfo = ContactInfo.Create("contact@company.com");

        // Assert
        Assert.Equal("contact@company.com", contactInfo.Email);
        Assert.Null(contactInfo.Phone);
        Assert.Null(contactInfo.Address);
    }

    /// <summary>
    /// Tests that ContactInfo trims whitespace.
    /// </summary>
    [Fact]
    public void Create_TrimsWhitespace()
    {
        // Act
        var contactInfo = ContactInfo.Create("  contact@company.com  ", "  +1234567890  ", "  123 Street  ");

        // Assert
        Assert.Equal("contact@company.com", contactInfo.Email);
        Assert.Equal("+1234567890", contactInfo.Phone);
        Assert.Equal("123 Street", contactInfo.Address);
    }

    /// <summary>
    /// Tests that ToString returns the email.
    /// </summary>
    [Fact]
    public void ToString_ReturnsEmail()
    {
        // Arrange
        var contactInfo = ContactInfo.Create("contact@company.com");

        // Act
        var result = contactInfo.ToString();

        // Assert
        Assert.Equal("contact@company.com", result);
    }
}