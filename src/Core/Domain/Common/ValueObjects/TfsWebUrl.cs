// <copyright file="TfsWebUrl.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Common.ValueObjects;

using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    /// Parses a JSON string (plain string or object {"value", "isValid"}) into a <see cref="TfsWebUrl"/>.
    /// Used by EF Core value converters for JSONB columns.
    /// </summary>
    /// <param name="json">The JSON string from the database column.</param>
    /// <returns>A <see cref="TfsWebUrl"/> instance, or null if the value is null/empty/invalid.</returns>
    public static TfsWebUrl? FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        var token = JToken.Parse(json);

        if (token.Type == JTokenType.String)
        {
            var value = token.Value<string>();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return TfsWebUrl.Create(value);
        }

        if (token.Type == JTokenType.Object)
        {
            var obj = (JObject)token;
            var valueToken = obj["value"];
            var isValidToken = obj["isValid"];

            if (valueToken == null || valueToken.Type == JTokenType.Null)
            {
                return null;
            }

            if (isValidToken != null && isValidToken.Type == JTokenType.Boolean && !isValidToken.Value<bool>())
            {
                return null;
            }

            var value = valueToken.Value<string>();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return TfsWebUrl.Create(value);
        }

        return null;
    }

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
/// JsonConverter for <see cref="TfsWebUrl"/> and nullable <see cref="TfsWebUrl"/> to handle JSON serialization/deserialization.
/// </summary>
public sealed class TfsWebUrlJsonConverter : JsonConverter
{
    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TfsWebUrl) || objectType == typeof(TfsWebUrl?);
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        // Handle both TfsWebUrl and nullable TfsWebUrl?
        // If value is TfsWebUrl with null Value, or TfsWebUrl? that is null, write null
        if (value is TfsWebUrl url)
        {
            if (url.Value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var obj = new JObject
                {
                    ["value"] = url.Value,
                    ["isValid"] = url.IsValid
                };
                obj.WriteTo(writer);
            }
        }
        else if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteNull();
        }
    }

    /// <inheritdoc />
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonToken.String)
        {
            var value = reader.Value?.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var url = TfsWebUrl.Create(value);
            return objectType == typeof(TfsWebUrl?) ? (TfsWebUrl?)url : url;
        }

        if (reader.TokenType == JsonToken.StartObject)
        {
            var obj = JObject.Load(reader);
            var valueToken = obj["value"];
            var isValidToken = obj["isValid"];

            // Rechazar formato envuelto {"tfsWebUrl": {"value": ..., "isValid": ...}}
            // como entrada de petición HTTP -> debe devolver Bad Request
            if (valueToken == null && obj["tfsWebUrl"] != null)
            {
                throw new JsonSerializationException(
                    "Formato JSON no válido para TfsWebUrl. Use un string plano o un objeto {\"value\": \"...\", \"isValid\": true}. No se admite el formato envuelto {\"tfsWebUrl\": {...}}.");
            }

            if (valueToken == null || valueToken.Type == JTokenType.Null)
            {
                return null;
            }

            if (isValidToken != null && isValidToken.Type == JTokenType.Boolean && !isValidToken.Value<bool>())
            {
                return null;
            }

            var value = valueToken.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var url = TfsWebUrl.Create(value);
            return objectType == typeof(TfsWebUrl?) ? (TfsWebUrl?)url : url;
        }

        return null;
    }
}
