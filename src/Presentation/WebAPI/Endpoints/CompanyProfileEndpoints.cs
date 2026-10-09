// <copyright file="CompanyProfileEndpoints.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Endpoints;

using Microsoft.AspNetCore.Mvc;
using Tfs.Portfolio.Application.CompanyProfile.Commands;
using Tfs.Portfolio.Application.CompanyProfile.Dtos;
using Tfs.Portfolio.Application.CompanyProfile.Queries;
using Tfs.Portfolio.Application.CompanyProfile.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.CompanyProfile.Repositories;
using AutoMapper;
using Tfs.Portfolio.Api.Filters;

/// <summary>
/// Company Profile endpoints (singleton resource).
/// </summary>
internal static class CompanyProfileEndpoints
{
    /// <summary>
    /// Maps the company profile endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapCompanyProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/company-profile").WithTags("CompanyProfile");

        group.MapGet("/", async ([FromServices] GetCompanyProfileQueryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetCompanyProfileQuery(), ct);
            return result is not null ? Results.Ok(result) : Results.Problem(
                title: "Not Found",
                detail: "Company profile has not been created yet.",
                statusCode: 404,
                instance: "/api/v1/company-profile");
        })
        .WithName("GetCompanyProfile")
        .Produces<CompanyProfileDto>()
        .Produces(404);

        group.MapPut("/", async (UpdateCompanyProfileCommand command, [FromServices] UpdateCompanyProfileCommandHandler handler, [FromServices] IMapper mapper, [FromServices] ICompanyProfileRepository companyProfileRepository, CancellationToken ct) =>
        {
            var existingProfile = await companyProfileRepository.GetAsync(ct);
            if (existingProfile is null)
            {
                return Results.Problem(
                    title: "Not Found",
                    detail: "Company profile has not been created yet.",
                    statusCode: 404,
                    instance: "/api/v1/company-profile");
            }

            await handler.HandleAsync(command, ct);
            var updatedProfile = await companyProfileRepository.GetAsync(ct);
            var profileDto = mapper.Map<CompanyProfileDto>(updatedProfile!);
            return Results.Ok(profileDto);
        })
        .WithName("UpdateCompanyProfile")
        .Accepts<UpdateCompanyProfileCommand>("application/json")
        .Produces<CompanyProfileDto>()
        .Produces<ProblemDetails>(400)
        .Produces(404)
        .AddEndpointFilter<ValidationFilter<UpdateCompanyProfileCommand>>();

        return endpoints;
    }
}