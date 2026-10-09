// <copyright file="CompanyProfileDtos.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.CompanyProfile.Dtos;

using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Data transfer object for a company profile.
/// </summary>
public sealed record CompanyProfileDto(
    Guid Id,
    string CompanyName,
    string ContactEmail,
    string? ContactPhone,
    TfsWebUrl? TfsWebUrl,
    string? Address,
    string? Description,
    TfsWebUrl? LogoUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Request to update an existing company profile.
/// </summary>
public sealed record UpdateCompanyProfileRequest(
    string CompanyName,
    string ContactEmail,
    string? ContactPhone,
    TfsWebUrl? TfsWebUrl,
    string? Address,
    string? Description,
    TfsWebUrl? LogoUrl
);