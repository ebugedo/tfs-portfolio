// <copyright file="TechnologyTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain.ValueObjects;

using Tfs.Portfolio.Domain.Common.ValueObjects;
using Xunit;

/// <summary>
/// Unit tests for <see cref="Technology"/> value object.
/// </summary>
public sealed class TechnologyTests
{
    /// <summary>
    /// Tests that Technology can be created with valid values.
    /// </summary>
    [Fact]
    public void Create_WithValidValues_ReturnsTechnology()
    {
        // Act
        var technology = Technology.Create("React", TechnologyCategory.Frontend, ProficiencyLevel.Expert);

        // Assert
        Assert.Equal("React", technology.Name);
        Assert.Equal(TechnologyCategory.Frontend, technology.Category);
        Assert.Equal(ProficiencyLevel.Expert, technology.Proficiency);
    }

    /// <summary>
    /// Tests that Technology creation fails with null name.
    /// </summary>
    [Fact]
    public void Create_WithNullName_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Technology.Create(null!, TechnologyCategory.Frontend, ProficiencyLevel.Beginner));
        Assert.Contains("Technology name cannot be null or whitespace", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Technology creation fails with whitespace name.
    /// </summary>
    [Fact]
    public void Create_WithWhitespaceName_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Technology.Create("   ", TechnologyCategory.Frontend, ProficiencyLevel.Beginner));
        Assert.Contains("Technology name cannot be null or whitespace", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Tests that Technology equality is case-insensitive on name.
    /// </summary>
    [Fact]
    public void Equality_CaseInsensitiveName_ReturnsTrue()
    {
        // Arrange
        var tech1 = Technology.Create("React", TechnologyCategory.Frontend, ProficiencyLevel.Expert);
        var tech2 = Technology.Create("react", TechnologyCategory.Backend, ProficiencyLevel.Beginner);

        // Act & Assert
        Assert.Equal(tech1, tech2);
        Assert.True(tech1 == tech2);
    }

    /// <summary>
    /// Tests that Technology inequality works for different names.
    /// </summary>
    [Fact]
    public void Equality_DifferentNames_ReturnsFalse()
    {
        // Arrange
        var tech1 = Technology.Create("React", TechnologyCategory.Frontend, ProficiencyLevel.Expert);
        var tech2 = Technology.Create("Vue", TechnologyCategory.Frontend, ProficiencyLevel.Expert);

        // Act & Assert
        Assert.NotEqual(tech1, tech2);
        Assert.True(tech1 != tech2);
    }

    /// <summary>
    /// Tests that Technology ToString returns correct format.
    /// </summary>
    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var technology = Technology.Create("React", TechnologyCategory.Frontend, ProficiencyLevel.Expert);

        // Act
        var result = technology.ToString();

        // Assert
        Assert.Equal("React (Frontend, Expert)", result);
    }
}