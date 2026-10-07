// <copyright file="SectorValidators.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Sectors.Validators;

using FluentValidation;
using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Application.Sectors.Queries;

/// <summary>
/// Validator for <see cref="CreateSectorCommand"/>.
/// </summary>
public sealed class CreateSectorCommandValidator : AbstractValidator<CreateSectorCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSectorCommandValidator"/> class.
    /// </summary>
    public CreateSectorCommandValidator()
    {
        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sector name is required")
            .MaximumLength(100).WithMessage("Sector name must not exceed 100 characters");

        this.RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Sector description must not exceed 500 characters")
            .When(x => x.Description is not null);
    }
}

/// <summary>
/// Validator for <see cref="UpdateSectorCommand"/>.
/// </summary>
public sealed class UpdateSectorCommandValidator : AbstractValidator<UpdateSectorCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSectorCommandValidator"/> class.
    /// </summary>
    public UpdateSectorCommandValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sector ID is required");

        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sector name is required")
            .MaximumLength(100).WithMessage("Sector name must not exceed 100 characters");

        this.RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Sector description must not exceed 500 characters")
            .When(x => x.Description is not null);
    }
}

/// <summary>
/// Validator for <see cref="GetSectorByIdQuery"/>.
/// </summary>
public sealed class GetSectorByIdQueryValidator : AbstractValidator<GetSectorByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetSectorByIdQueryValidator"/> class.
    /// </summary>
    public GetSectorByIdQueryValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sector ID is required");
    }
}