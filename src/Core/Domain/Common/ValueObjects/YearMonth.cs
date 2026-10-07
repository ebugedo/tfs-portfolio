// <copyright file="YearMonth.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Common.ValueObjects;

using System;

/// <summary>
/// Represents a year and month combination (e.g., June 2024).
/// Immutable value object with no day component.
/// </summary>
public readonly struct YearMonth : IEquatable<YearMonth>, IComparable<YearMonth>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YearMonth"/> struct.
    /// </summary>
    /// <param name="month">The month (1-12).</param>
    /// <param name="year">The year.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not between 1 and 12.</exception>
    public YearMonth(int month, int year)
    {
        if (month < 1 || month > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
        }

        this.Month = month;
        this.Year = year;
    }

    /// <summary>
    /// Gets the month (1-12).
    /// </summary>
    public int Month { get; }

    /// <summary>
    /// Gets the year (e.g., 2024).
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Compares two <see cref="YearMonth"/> instances for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if both month and year are equal.</returns>
    public static bool operator ==(YearMonth left, YearMonth right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="YearMonth"/> instances for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if month or year are not equal.</returns>
    public static bool operator !=(YearMonth left, YearMonth right) => !left.Equals(right);

    /// <summary>
    /// Compares two <see cref="YearMonth"/> instances for ordering.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if left is less than right.</returns>
    public static bool operator <(YearMonth left, YearMonth right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Compares two <see cref="YearMonth"/> instances for ordering.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if left is greater than right.</returns>
    public static bool operator >(YearMonth left, YearMonth right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Compares two <see cref="YearMonth"/> instances for ordering.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if left is less than or equal to right.</returns>
    public static bool operator <=(YearMonth left, YearMonth right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Compares two <see cref="YearMonth"/> instances for ordering.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if left is greater than or equal to right.</returns>
    public static bool operator >=(YearMonth left, YearMonth right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Creates a new <see cref="YearMonth"/> instance.
    /// </summary>
    /// <param name="month">The month (1-12).</param>
    /// <param name="year">The year.</param>
    /// <returns>A new <see cref="YearMonth"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when month is not between 1 and 12.</exception>
    public static YearMonth Create(int month, int year) => new(month, year);

    /// <summary>
    /// Creates a <see cref="YearMonth"/> from a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="dateTime">The date time to extract month and year from.</param>
    /// <returns>A new <see cref="YearMonth"/> instance.</returns>
    public static YearMonth FromDateTime(DateTime dateTime) => new(dateTime.Month, dateTime.Year);

    /// <summary>
    /// Adds the specified number of months to this instance.
    /// </summary>
    /// <param name="months">The number of months to add (can be negative).</param>
    /// <returns>A new <see cref="YearMonth"/> with the months added.</returns>
    public YearMonth AddMonths(int months)
    {
        var totalMonths = (this.Year * 12) + (this.Month - 1) + months;
        var newYear = totalMonths / 12;
        var newMonth = (totalMonths % 12) + 1;
        return new YearMonth(newMonth, newYear);
    }

    /// <summary>
    /// Determines whether the current instance is equal to another instance.
    /// </summary>
    /// <param name="other">The other YearMonth to compare.</param>
    /// <returns>True if both month and year are equal.</returns>
    public bool Equals(YearMonth other) => this.Month == other.Month && this.Year == other.Year;

    /// <summary>
    /// Determines whether the current instance is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if obj is a YearMonth with equal month and year.</returns>
    public override bool Equals(object? obj) => obj is YearMonth other && this.Equals(other);

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(this.Month, this.Year);

    /// <summary>
    /// Compares the current instance with another YearMonth.
    /// </summary>
    /// <param name="other">The YearMonth to compare with.</param>
    /// <returns>A value indicating the relative order.</returns>
    public int CompareTo(YearMonth other)
    {
        var yearComparison = this.Year.CompareTo(other.Year);
        return yearComparison != 0 ? yearComparison : this.Month.CompareTo(other.Month);
    }

    /// <summary>
    /// Returns a string representation in "MM/yyyy" format.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public override string ToString() => $"{this.Month:D2}/{this.Year}";
}