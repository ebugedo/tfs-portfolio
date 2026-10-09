// <copyright file="TfsWebUrl.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Common.ValueObjects;

using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

/// <summary>
/// Represents a validated URL.
/// Immutable value object with implicit conversion from string and nullable support.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Conversion operators follow operators per StyleCop SA1201 documented order.")]
[Newtonsoft.Json.JsonConverter(typeof(TfsWebUrlJsonConverter))]
public readonly struct TfsWebUrl : IEquatable<TfsWebUrl>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TfsWebUrl"/> struct.
    /// </summary>
    /// <param name="value">The URL string.</param>
    private TfsWebUrl(string? value)
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
    /// Compares two <see cref="TfsWebUrl"/> instances for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if both values are equal.</returns>
    public static bool operator ==(TfsWebUrl left, TfsWebUrl right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="TfsWebUrl"/> instances for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if values are not equal.</returns>
    public static bool operator !=(TfsWebUrl left, TfsWebUrl right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a string to a <see cref="TfsWebUrl"/> (allows null/empty).
    /// </summary>
    /// <param name="value">The URL string.</param>
    public static implicit operator TfsWebUrl(string? value) => new(value);

    /// <summary>
    /// Implicitly converts a <see cref="TfsWebUrl"/> to a string.
    /// </summary>
    /// <param name="url">The URL.</param>
    public static implicit operator string?(TfsWebUrl url) => url.Value;

    /// <summary>
    /// Creates a new <see cref="TfsWebUrl"/> instance with validation.
    /// </summary>
    /// <param name="value">The URL string.</param>
    /// <returns>A new <see cref="TfsWebUrl"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when value is not a valid HTTP/HTTPS URL.</exception>
    public static TfsWebUrl Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new TfsWebUrl(null);
        }

        var trimmed = value.Trim();
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Invalid URL format. Must be a valid HTTP or HTTPS URL.", nameof(value));
        }

        return new TfsWebUrl(trimmed);
    }

    /// <summary>
    /// Creates a new <see cref="TfsWebUrl"/> instance without validation (for trusted sources).
    /// </summary>
    /// <param name="value">The URL string.</param>
    /// <returns>A new <see cref="TfsWebUrl"/> instance.</returns>
    public static TfsWebUrl FromString(string? value) => new(value);

    /// <summary>
    /// Determines whether the current instance is equal to another instance.
    /// </summary>
    /// <param name="other">The other TfsWebUrl to compare.</param>
    /// <returns>True if both values are equal.</returns>
    public bool Equals(TfsWebUrl other) => string.Equals(this.Value, other.Value, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the current instance is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if obj is a TfsWebUrl with equal value.</returns>
    public override bool Equals(object? obj) => obj is TfsWebUrl other && this.Equals(other);

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

/// <summary>
/// JsonConverter for <see cref="TfsWebUrl"/> to handle JSON serialization/deserialization.
/// </summary>
public sealed class TfsWebUrlJsonConverter : JsonConverter<TfsWebUrl>
{
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, TfsWebUrl value, JsonSerializer serializer)
    {
        writer.WriteValue(value.Value);
    }

    /// <inheritdoc />
    public override TfsWebUrl ReadJson(JsonReader reader, Type objectType, TfsWebUrl existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return default;
        }

        var value = reader.Value?.ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }

        return TfsWebUrl.Create(value);
    }
}