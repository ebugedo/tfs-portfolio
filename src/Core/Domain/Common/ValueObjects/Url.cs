// <copyright file="Url.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Common.ValueObjects;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a validated URL.
/// Immutable value object with implicit conversion from string and nullable support.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Conversion operators follow operators per StyleCop SA1201 documented order.")]
public readonly struct Url : IEquatable<Url>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> struct.
    /// </summary>
    /// <param name="value">The URL string.</param>
    private Url(string? value)
    {
        this.Value = value?.Trim();
    }

    /// <summary>
    /// Gets the URL value.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Gets a value indicating whether the URL is valid (non-null and valid format).
    /// </summary>
    public bool IsValid => this.Value is not null && Uri.TryCreate(this.Value, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// Compares two <see cref="Url"/> instances for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if both values are equal.</returns>
    public static bool operator ==(Url left, Url right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="Url"/> instances for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if values are not equal.</returns>
    public static bool operator !=(Url left, Url right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a string to a <see cref="Url"/> (allows null/empty).
    /// </summary>
    /// <param name="value">The URL string.</param>
    public static implicit operator Url(string? value) => new(value);

    /// <summary>
    /// Implicitly converts a <see cref="Url"/> to a string.
    /// </summary>
    /// <param name="url">The URL.</param>
    public static implicit operator string?(Url url) => url.Value;

    /// <summary>
    /// Creates a new <see cref="Url"/> instance with validation.
    /// </summary>
    /// <param name="value">The URL string.</param>
    /// <returns>A new <see cref="Url"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when value is not a valid HTTP/HTTPS URL.</exception>
    public static Url Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Url(null);
        }

        var trimmed = value.Trim();
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Invalid URL format. Must be a valid HTTP or HTTPS URL.", nameof(value));
        }

        return new Url(trimmed);
    }

    /// <summary>
    /// Creates a new <see cref="Url"/> instance without validation (for trusted sources).
    /// </summary>
    /// <param name="value">The URL string.</param>
    /// <returns>A new <see cref="Url"/> instance.</returns>
    public static Url FromString(string? value) => new(value);

    /// <summary>
    /// Determines whether the current instance is equal to another instance.
    /// </summary>
    /// <param name="other">The other Url to compare.</param>
    /// <returns>True if both values are equal.</returns>
    public bool Equals(Url other) => string.Equals(this.Value, other.Value, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the current instance is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if obj is a Url with equal value.</returns>
    public override bool Equals(object? obj) => obj is Url other && this.Equals(other);

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => this.Value?.GetHashCode(StringComparison.Ordinal) ?? 0;

    /// <summary>
    /// Returns a string representation.
    /// </summary>
    /// <returns>The URL value or empty string if null.</returns>
    public override string ToString() => this.Value ?? string.Empty;
}