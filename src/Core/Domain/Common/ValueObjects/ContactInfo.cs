// <copyright file="ContactInfo.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Common.ValueObjects;

using System;
using System.Net.Mail;

/// <summary>
/// Represents contact information with email, phone, and address.
/// Immutable value object with email validation.
/// </summary>
public readonly struct ContactInfo : IEquatable<ContactInfo>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContactInfo"/> struct.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="phone">The phone number.</param>
    /// <param name="address">The address.</param>
    /// <exception cref="ArgumentException">Thrown when email is not a valid format.</exception>
    public ContactInfo(string email, string? phone, string? address)
    {
        var trimmedEmail = email?.Trim();
        if (string.IsNullOrWhiteSpace(trimmedEmail) || !IsValidEmail(trimmedEmail))
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }

        this.Email = trimmedEmail!;
        this.Phone = phone?.Trim();
        this.Address = address?.Trim();
    }

    /// <summary>
    /// Gets the email address.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Gets the phone number.
    /// </summary>
    public string? Phone { get; }

    /// <summary>
    /// Gets the address.
    /// </summary>
    public string? Address { get; }

    /// <summary>
    /// Compares two <see cref="ContactInfo"/> instances for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if all properties are equal.</returns>
    public static bool operator ==(ContactInfo left, ContactInfo right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="ContactInfo"/> instances for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if any property is not equal.</returns>
    public static bool operator !=(ContactInfo left, ContactInfo right) => !left.Equals(right);

    /// <summary>
    /// Creates a new <see cref="ContactInfo"/> instance.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="phone">The phone number.</param>
    /// <param name="address">The address.</param>
    /// <returns>A new <see cref="ContactInfo"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when email is not a valid format.</exception>
    public static ContactInfo Create(string email, string? phone = null, string? address = null)
        => new(email, phone, address);

    /// <summary>
    /// Determines whether the current instance is equal to another instance.
    /// </summary>
    /// <param name="other">The other ContactInfo to compare.</param>
    /// <returns>True if all properties are equal.</returns>
    public bool Equals(ContactInfo other)
        => string.Equals(this.Email, other.Email, StringComparison.OrdinalIgnoreCase)
            && string.Equals(this.Phone, other.Phone, StringComparison.Ordinal)
            && string.Equals(this.Address, other.Address, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the current instance is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if obj is a ContactInfo with equal properties.</returns>
    public override bool Equals(object? obj) => obj is ContactInfo other && this.Equals(other);

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
        => HashCode.Combine(
            this.Email?.GetHashCode(StringComparison.OrdinalIgnoreCase),
            this.Phone?.GetHashCode(StringComparison.Ordinal),
            this.Address?.GetHashCode(StringComparison.Ordinal));

    /// <summary>
    /// Returns a string representation.
    /// </summary>
    /// <returns>The email address.</returns>
    public override string ToString() => this.Email;

    /// <summary>
    /// Validates email format using System.Net.Mail.MailAddress.
    /// </summary>
    /// <param name="email">The email to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}