// <copyright file="CompanyProfileQueries.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.CompanyProfile.Queries;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.CompanyProfile.Dtos;

/// <summary>
/// Query to get the company profile.
/// </summary>
public sealed record GetCompanyProfileQuery
    : IQuery<CompanyProfileDto>;