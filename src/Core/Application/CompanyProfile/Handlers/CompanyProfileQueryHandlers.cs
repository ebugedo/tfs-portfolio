// <copyright file="CompanyProfileQueryHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.CompanyProfile.Handlers;

using AutoMapper;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.CompanyProfile.Dtos;
using Tfs.Portfolio.Application.CompanyProfile.Queries;
using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Domain.CompanyProfile.Repositories;

/// <summary>
/// Handler for <see cref="GetCompanyProfileQuery"/>.
/// </summary>
public sealed class GetCompanyProfileQueryHandler : QueryHandlerBase<GetCompanyProfileQuery, CompanyProfileDto>
{
    private readonly ICompanyProfileRepository companyProfileRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCompanyProfileQueryHandler"/> class.
    /// </summary>
    /// <param name="companyProfileRepository">The company profile repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetCompanyProfileQueryHandler(
        ICompanyProfileRepository companyProfileRepository,
        IMapper mapper)
    {
        this.companyProfileRepository = companyProfileRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<CompanyProfileDto> HandleAsync(GetCompanyProfileQuery query, CancellationToken cancellationToken = default)
    {
        var profile = await this.companyProfileRepository.GetAsync(cancellationToken);
        if (profile is null)
        {
            return null!;
        }
        return this.mapper.Map<CompanyProfileDto>(profile);
    }
}