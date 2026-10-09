// <copyright file="CompanyProfileValidators.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.CompanyProfile.Validators;

using FluentValidation;
using Tfs.Portfolio.Application.CompanyProfile.Commands;
using Tfs.Portfolio.Application.CompanyProfile.Queries;

/// <summary>
/// Validator for <see cref="UpdateCompanyProfileCommand"/>.
/// </summary>
public sealed class UpdateCompanyProfileCommandValidator : AbstractValidator<UpdateCompanyProfileCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCompanyProfileCommandValidator"/> class.
    /// </summary>
    public UpdateCompanyProfileCommandValidator()
    {
        this.RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters");

        this.RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required")
            .EmailAddress().WithMessage("Invalid contact email format");

        this.RuleFor(x => x.TfsWebUrl)
            .Must(url => !url.HasValue || url.Value.IsValid).WithMessage("Invalid web URL format")
            .When(x => x.TfsWebUrl.HasValue);

        this.RuleFor(x => x.LogoUrl)
            .Must(url => !url.HasValue || url.Value.IsValid).WithMessage("Invalid logo URL format")
            .When(x => x.LogoUrl.HasValue);
    }
}

/// <summary>
/// Validator for <see cref="GetCompanyProfileQuery"/>.
/// </summary>
public sealed class GetCompanyProfileQueryValidator : AbstractValidator<GetCompanyProfileQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCompanyProfileQueryValidator"/> class.
    /// </summary>
    public GetCompanyProfileQueryValidator()
    {
        // No validation needed for singleton query
    }
}