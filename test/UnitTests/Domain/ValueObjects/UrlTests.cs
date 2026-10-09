// <copyright file="TfsWebUrlTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain.ValueObjects;

using Tfs.Portfolio.Domain.Common.ValueObjects;
using Xunit;

/// <summary>
/// Unit tests for <see cref="TfsWebUrl"/> value object.
/// </summary>
public sealed class TfsWebUrlTests
{
    /// <summary>
    /// Tests that TfsWebUrl can be created with valid HTTP URL.
    /// </summary>
    [Fact]
    public void Create_WithValidHttpUrl_ReturnsUrl()
    {
        // Act
        var url = TfsWebUrl.Create("http://example.com/logo.png");

        // Assert
        Assert.Equal("http://example.com/logo.png", url.Value);
        Assert.True(url.IsValid);
    }

    /// <summary>
    /// Tests that TfsWebUrl can be created with valid HTTPS URL.
    /// </summary>
    [Fact]
    public void Create_WithValidHttpsUrl_ReturnsUrl()
    {
        // Act
        var url = TfsWebUrl.Create("https://example.com/logo.png");

        // Assert
        Assert.Equal("https://example.com/logo.png", url.Value);
        Assert.True(url.IsValid);
    }

    /// <summary>
    /// Tests that TfsWebUrl creation fails with invalid URL format.
    /// </summary>
    [Fact]
    public void Create_WithInvalidUrl_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TfsWebUrl.Create("not-a-url"));
        Assert.Contains("Invalid URL format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that TfsWebUrl creation fails with FTP URL (not HTTP/HTTPS).
    /// </summary>
    [Fact]
    public void Create_WithFtpUrl_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TfsWebUrl.Create("ftp://example.com/file.txt"));
        Assert.Contains("Invalid URL format", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that TfsWebUrl handles null gracefully.
    /// </summary>
    [Fact]
    public void Create_WithNull_ReturnsInvalidUrl()
    {
        // Act
        var url = TfsWebUrl.Create(null);

        // Assert
        Assert.Null(url.Value);
        Assert.False(url.IsValid);
    }

    /// <summary>
    /// Tests that TfsWebUrl handles empty string gracefully.
    /// </summary>
    [Fact]
    public void Create_WithEmptyString_ReturnsInvalidUrl()
    {
        // Act
        var url = TfsWebUrl.Create(string.Empty);

        // Assert
        Assert.Null(url.Value);
        Assert.False(url.IsValid);
    }

    /// <summary>
    /// Tests that TfsWebUrl handles whitespace gracefully.
    /// </summary>
    [Fact]
    public void Create_WithWhitespace_ReturnsInvalidUrl()
    {
        // Act
        var url = TfsWebUrl.Create("   ");

        // Assert
        Assert.Null(url.Value);
        Assert.False(url.IsValid);
    }

    /// <summary>
    /// Tests that implicit conversion from string works.
    /// </summary>
    [Fact]
    public void ImplicitConversion_FromString_Works()
    {
        // Act
        TfsWebUrl url = "https://example.com";

        // Assert
        Assert.Equal("https://example.com", url.Value);
        Assert.True(url.IsValid);
    }

    /// <summary>
    /// Tests that implicit conversion to string works.
    /// </summary>
    [Fact]
    public void ImplicitConversion_ToString_Works()
    {
        // Arrange
        var url = TfsWebUrl.Create("https://example.com");

        // Act
        string? result = url;

        // Assert
        Assert.Equal("https://example.com", result);
    }

    /// <summary>
    /// Tests that ToString returns the URL value.
    /// </summary>
    [Fact]
    public void ToString_ReturnsUrlValue()
    {
        // Arrange
        var url = TfsWebUrl.Create("https://example.com/logo.png");

        // Act
        var result = url.ToString();

        // Assert
        Assert.Equal("https://example.com/logo.png", result);
    }

    /// <summary>
    /// Tests that ToString returns empty string for invalid URL.
    /// </summary>
    [Fact]
    public void ToString_ForInvalidUrl_ReturnsEmptyString()
    {
        // Arrange
        var url = TfsWebUrl.Create(null);

        // Act
        var result = url.ToString();

        // Assert
        Assert.Equal(string.Empty, result);
    }
}