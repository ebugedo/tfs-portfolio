// <copyright file="CompanyProfile.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.CompanyProfile.Entities;

using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.CompanyProfile.Events;
using Tfs.Portfolio.Domain.CompanyProfile.Exceptions;

/// <summary>
/// Represents the singleton company profile aggregate root.
/// Only one instance can exist in the system.
/// </summary>
public sealed class CompanyProfileEntity : AggregateRoot<Guid>
{
    private static CompanyProfileEntity? _instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileEntity"/> class.
    /// </summary>
    private CompanyProfileEntity()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileEntity"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="companyName">The company name.</param>
    /// <param name="contactEmail">The contact email.</param>
    /// <param name="contactPhone">The contact phone number.</param>
    /// <param name="webUrl">The company web URL.</param>
    /// <param name="address">The company address.</param>
    /// <param name="description">The company description.</param>
    /// <param name="logoUrl">The company logo URL.</param>
    /// <param name="createdAt">The creation date.</param>
    private CompanyProfileEntity(Guid id, string companyName, string contactEmail, string? contactPhone, TfsWebUrl? webUrl, string? address, string? description, TfsWebUrl? logoUrl, DateTime createdAt)
        : base(id)
    {
        this.CompanyName = companyName;
        this.ContactEmail = contactEmail;
        this.ContactPhone = contactPhone;
        this.TfsWebUrl = webUrl;
        this.Address = address;
        this.Description = description;
        this.LogoUrl = logoUrl;
        this.CreatedAt = createdAt;
    }

    /// <summary>
    /// Gets the existing company profile instance.
    /// </summary>
    public static CompanyProfileEntity? Instance => _instance;

    /// <summary>
    /// Gets the company name.
    /// </summary>
    public string CompanyName { get; private set; } = default!;

    /// <summary>
    /// Gets the contact email.
    /// </summary>
    public string ContactEmail { get; private set; } = default!;

    /// <summary>
    /// Gets the contact phone number.
    /// </summary>
    public string? ContactPhone { get; private set; }

    /// <summary>
    /// Gets the company web URL.
    /// </summary>
    public TfsWebUrl? TfsWebUrl { get; private set; }

    /// <summary>
    /// Gets the company address.
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// Gets the company description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the company logo URL.
    /// </summary>
    public TfsWebUrl? LogoUrl { get; private set; }

    /// <summary>
    /// Gets the date and time when the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the profile was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new company profile (singleton).
    /// </summary>
    /// <param name="companyName">The company name.</param>
    /// <param name="contactEmail">The contact email.</param>
    /// <param name="contactPhone">The contact phone number.</param>
    /// <param name="webUrl">The company web URL.</param>
    /// <param name="address">The company address.</param>
    /// <param name="description">The company description.</param>
    /// <param name="logoUrl">The company logo URL.</param>
    /// <returns>A new company profile instance.</returns>
    /// <exception cref="CompanyProfileAlreadyExistsException">Thrown when a profile already exists.</exception>
    /// <exception cref="InvalidCompanyProfileStateException">Thrown when validation fails.</exception>
    public static CompanyProfileEntity Create(string companyName, string contactEmail, string? contactPhone = null, TfsWebUrl? webUrl = null, string? address = null, string? description = null, TfsWebUrl? logoUrl = null)
    {
        if (_instance != null)
        {
            throw new CompanyProfileAlreadyExistsException("Company profile already exists");
        }

        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new InvalidCompanyProfileStateException("Company name cannot be empty");
        }

        var trimmedCompanyName = companyName.Trim();
        if (trimmedCompanyName.Length > 200)
        {
            throw new InvalidCompanyProfileStateException("Company name must not exceed 200 characters");
        }

        var trimmedContactEmail = contactEmail.Trim();
        if (!IsValidEmail(trimmedContactEmail))
        {
            throw new InvalidCompanyProfileStateException("Invalid contact email format");
        }

        if (webUrl is { IsValid: false })
        {
            throw new InvalidCompanyProfileStateException("Invalid web URL format");
        }

        if (logoUrl is { IsValid: false })
        {
            throw new InvalidCompanyProfileStateException("Invalid logo URL format");
        }

        var profile = new CompanyProfileEntity(
            Guid.NewGuid(),
            companyName.Trim(),
            trimmedContactEmail,
            contactPhone?.Trim(),
            webUrl,
            address?.Trim(),
            description?.Trim(),
            logoUrl,
            DateTime.UtcNow);

        _instance = profile;
        profile.AddDomainEvent(new CompanyProfileCreatedEvent(profile.Id, profile.CompanyName));
        return profile;
    }

    /// <summary>
    /// Resets the singleton instance (for testing purposes only).
    /// </summary>
    public static void ResetInstance() => _instance = null;

    /// <summary>
    /// Creates a new company profile (singleton).
    /// </summary>
    /// <param name="companyName">The new company name.</param>
    /// <param name="contactEmail">The new contact email.</param>
    /// <param name="contactPhone">The new contact phone number.</param>
    /// <param name="webUrl">The new web URL.</param>
    /// <param name="address">The new address.</param>
    /// <param name="description">The new description.</param>
    /// <param name="logoUrl">The new logo URL.</param>
    /// <exception cref="InvalidCompanyProfileStateException">Thrown when validation fails.</exception>
    public void Update(string companyName, string contactEmail, string? contactPhone = null, TfsWebUrl? webUrl = null, string? address = null, string? description = null, TfsWebUrl? logoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new InvalidCompanyProfileStateException("Company name cannot be empty");
        }

        var trimmedCompanyName = companyName.Trim();
        if (trimmedCompanyName.Length > 200)
        {
            throw new InvalidCompanyProfileStateException("Company name must not exceed 200 characters");
        }

        var trimmedContactEmail = contactEmail.Trim();
        if (!IsValidEmail(trimmedContactEmail))
        {
            throw new InvalidCompanyProfileStateException("Invalid contact email format");
        }

        if (webUrl is { IsValid: false })
        {
            throw new InvalidCompanyProfileStateException("Invalid web URL format");
        }

        if (logoUrl is { IsValid: false })
        {
            throw new InvalidCompanyProfileStateException("Invalid logo URL format");
        }

        this.CompanyName = companyName.Trim();
        this.ContactEmail = contactEmail.Trim();
        this.ContactPhone = contactPhone?.Trim();
        this.TfsWebUrl = webUrl;
        this.Address = address?.Trim();
        this.Description = description?.Trim();
        this.LogoUrl = logoUrl;
        this.UpdatedAt = DateTime.UtcNow;
        this.AddDomainEvent(new CompanyProfileUpdatedEvent(this.Id));
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
