// <copyright file="ValueObjectTests.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Domain;

using FluentAssertions;
using Tfs.Portfolio.Domain.Common;
using Xunit;

/// <summary>
/// Test value object for testing equality.
/// </summary>
public sealed record TestValueObject(string Value1, int Value2) : ValueObject;

/// <summary>
/// Unit tests for the ValueObject base class.
/// </summary>
public class ValueObjectTests
{
    /// <summary>
    /// Tests that two value objects with the same values are equal.
    /// </summary>
    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var obj1 = new TestValueObject("Test", 42);
        var obj2 = new TestValueObject("Test", 42);

        // Act & Assert
        obj1.Should().Be(obj2);
        (obj1 == obj2).Should().BeTrue();
        (obj1 != obj2).Should().BeFalse();
    }

    /// <summary>
    /// Tests that two value objects with different values are not equal.
    /// </summary>
    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var obj1 = new TestValueObject("Test", 42);
        var obj2 = new TestValueObject("Different", 42);

        // Act & Assert
        obj1.Should().NotBe(obj2);
        (obj1 == obj2).Should().BeFalse();
        (obj1 != obj2).Should().BeTrue();
    }

    /// <summary>
    /// Tests that a value object is not equal to null.
    /// </summary>
    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var obj = new TestValueObject("Test", 42);

        // Act & Assert
        obj.Equals(null).Should().BeFalse();
        (obj == null).Should().BeFalse();
        (null == obj).Should().BeFalse();
    }
}