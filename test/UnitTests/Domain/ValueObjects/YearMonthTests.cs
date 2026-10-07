// <copyright file="YearMonthTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain.ValueObjects;

using Tfs.Portfolio.Domain.Common.ValueObjects;
using Xunit;

/// <summary>
/// Unit tests for <see cref="YearMonth"/> value object.
/// </summary>
public sealed class YearMonthTests
{
    /// <summary>
    /// Tests that YearMonth can be created with valid values.
    /// </summary>
    [Fact]
    public void Create_WithValidValues_ReturnsYearMonth()
    {
        // Act
        var yearMonth = YearMonth.Create(6, 2024);

        // Assert
        Assert.Equal(6, yearMonth.Month);
        Assert.Equal(2024, yearMonth.Year);
    }

    /// <summary>
    /// Tests that YearMonth creation fails with month less than 1.
    /// </summary>
    [Fact]
    public void Create_WithMonthLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => YearMonth.Create(0, 2024));
        Assert.Contains("Month must be between 1 and 12", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that YearMonth creation fails with month greater than 12.
    /// </summary>
    [Fact]
    public void Create_WithMonthGreaterThanTwelve_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => YearMonth.Create(13, 2024));
        Assert.Contains("Month must be between 1 and 12", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that YearMonth can be created from DateTime.
    /// </summary>
    [Fact]
    public void FromDateTime_ReturnsCorrectYearMonth()
    {
        // Arrange
        var dateTime = new DateTime(2024, 6, 15);

        // Act
        var yearMonth = YearMonth.FromDateTime(dateTime);

        // Assert
        Assert.Equal(6, yearMonth.Month);
        Assert.Equal(2024, yearMonth.Year);
    }

    /// <summary>
    /// Tests that AddMonths works correctly within the same year.
    /// </summary>
    [Fact]
    public void AddMonths_WithinSameYear_ReturnsCorrectYearMonth()
    {
        // Arrange
        var yearMonth = YearMonth.Create(6, 2024);

        // Act
        var result = yearMonth.AddMonths(3);

        // Assert
        Assert.Equal(9, result.Month);
        Assert.Equal(2024, result.Year);
    }

    /// <summary>
    /// Tests that AddMonths works correctly across year boundary.
    /// </summary>
    [Fact]
    public void AddMonths_AcrossYearBoundary_ReturnsCorrectYearMonth()
    {
        // Arrange
        var yearMonth = YearMonth.Create(11, 2024);

        // Act
        var result = yearMonth.AddMonths(3);

        // Assert
        Assert.Equal(2, result.Month);
        Assert.Equal(2025, result.Year);
    }

    /// <summary>
    /// Tests that AddMonths works correctly with negative values.
    /// </summary>
    [Fact]
    public void AddMonths_WithNegativeValue_ReturnsCorrectYearMonth()
    {
        // Arrange
        var yearMonth = YearMonth.Create(3, 2024);

        // Act
        var result = yearMonth.AddMonths(-5);

        // Assert
        Assert.Equal(10, result.Month);
        Assert.Equal(2023, result.Year);
    }

    /// <summary>
    /// Tests that YearMonth equality works correctly for same values.
    /// </summary>
    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var ym1 = YearMonth.Create(6, 2024);
        var ym2 = YearMonth.Create(6, 2024);

        // Act & Assert
        Assert.Equal(ym1, ym2);
        Assert.True(ym1 == ym2);
    }

    /// <summary>
    /// Tests that YearMonth equality works correctly for different values.
    /// </summary>
    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var ym1 = YearMonth.Create(6, 2024);
        var ym2 = YearMonth.Create(7, 2024);

        // Act & Assert
        Assert.NotEqual(ym1, ym2);
        Assert.True(ym1 != ym2);
    }

    /// <summary>
    /// Tests that YearMonth comparison operators work correctly.
    /// </summary>
    [Fact]
    public void ComparisonOperators_WorkCorrectly()
    {
        // Arrange
        var ym1 = YearMonth.Create(1, 2024);
        var ym2 = YearMonth.Create(6, 2024);
        var ym3 = YearMonth.Create(1, 2025);

        // Act & Assert
        Assert.True(ym1 < ym2);
        Assert.True(ym2 > ym1);
        Assert.True(ym2 < ym3);
        Assert.True(ym3 > ym2);
        Assert.True(ym1 <= ym2);
        Assert.True(ym2 >= ym1);
    }

    /// <summary>
    /// Tests that ToString returns correct format.
    /// </summary>
    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var yearMonth = YearMonth.Create(6, 2024);

        // Act
        var result = yearMonth.ToString();

        // Assert
        Assert.Equal("06/2024", result);
    }
}