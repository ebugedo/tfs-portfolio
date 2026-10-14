// <copyright file="CompanyProfileController.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Controllers;

using Tfs.Portfolio.Application.CompanyProfile.Commands;
using Tfs.Portfolio.Application.CompanyProfile.Dtos;
using Tfs.Portfolio.Application.CompanyProfile.Queries;
using Tfs.Portfolio.Application.CompanyProfile.Handlers;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Domain.CompanyProfile.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing company profile (singleton resource).
/// </summary>
[ApiController]
[Route("api/v1/company-profile")]
[Tags("CompanyProfile")]
public sealed class CompanyProfileController : ControllerBase
{
    /// <summary>
    /// Gets the company profile.
    /// </summary>
    /// <param name="handler">The query handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The company profile if found.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompanyProfile(
        [FromServices] GetCompanyProfileQueryHandler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetCompanyProfileQuery(), ct);
        return result is not null ? Ok(result) : NotFound(new ProblemDetails
        {
            Title = "Not Found",
            Detail = "Company profile has not been created yet.",
            Status = StatusCodes.Status404NotFound,
            Instance = "/api/v1/company-profile"
        });
    }

    /// <summary>
    /// Creates or updates the company profile.
    /// </summary>
    /// <param name="command">The update company profile command.</param>
    /// <param name="handler">The command handler.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="companyProfileRepository">The company profile repository.</param>
    /// <param name="validator">The validator for the command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated company profile.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompanyProfile(
        [FromBody] UpdateCompanyProfileCommand command,
        [FromServices] UpdateCompanyProfileCommandHandler handler,
        [FromServices] IMapper mapper,
        [FromServices] ICompanyProfileRepository companyProfileRepository,
        [FromServices] IValidator<UpdateCompanyProfileCommand> validator,
        CancellationToken ct)
    {
        // Validate before checking existence
        var validationResult = await validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "One or more validation errors occurred.",
                Extensions = { ["errors"] = validationResult.ToDictionary() }
            };
            return BadRequest(problemDetails);
        }

        var existingProfile = await companyProfileRepository.GetAsync(ct);
        if (existingProfile is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Not Found",
                Detail = "Company profile has not been created yet.",
                Status = StatusCodes.Status404NotFound,
                Instance = "/api/v1/company-profile"
            });
        }

        await handler.HandleAsync(command, ct);
        var updatedProfile = await companyProfileRepository.GetAsync(ct);
        var profileDto = mapper.Map<CompanyProfileDto>(updatedProfile!);
        return Ok(profileDto);
    }
}