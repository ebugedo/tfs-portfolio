// <copyright file="CompanyProfileCommands.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.CompanyProfile.Commands;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Command to update an existing company profile.
/// </summary>
public sealed record UpdateCompanyProfileCommand(
    string CompanyName,
    string ContactEmail,
    string? ContactPhone,
    Url? WebUrl,
    string? Address,
    string? Description,
    Url? LogoUrl
) : ICommand;