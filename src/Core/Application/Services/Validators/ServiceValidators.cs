// <copyright file="ServiceValidators.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Services.Validators;

using FluentValidation;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Services.Queries;
using Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// Validator for <see cref="CreateServiceCommand"/>.
/// </summary>
public sealed class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateServiceCommandValidator"/> class.
    /// </summary>
    public CreateServiceCommandValidator()
    {
        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Service name is required")
            .MaximumLength(150).WithMessage("Service name must not exceed 150 characters");

        this.RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Service description must not exceed 1000 characters")
            .When(x => x.Description is not null);

        this.RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid service category");
    }
}

/// <summary>
/// Validator for <see cref="UpdateServiceCommand"/>.
/// </summary>
public sealed class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateServiceCommandValidator"/> class.
    /// </summary>
    public UpdateServiceCommandValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Service ID is required");

        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Service name is required")
            .MaximumLength(150).WithMessage("Service name must not exceed 150 characters");

        this.RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Service description must not exceed 1000 characters")
            .When(x => x.Description is not null);

        this.RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid service category");
    }
}

/// <summary>
/// Validator for <see cref="GetServiceByIdQuery"/>.
/// </summary>
public sealed class GetServiceByIdQueryValidator : AbstractValidator<GetServiceByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetServiceByIdQueryValidator"/> class.
    /// </summary>
    public GetServiceByIdQueryValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Service ID is required");
    }
}

/// <summary>
/// Validator for <see cref="GetServicesByCategoryQuery"/>.
/// </summary>
public sealed class GetServicesByCategoryQueryValidator : AbstractValidator<GetServicesByCategoryQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetServicesByCategoryQueryValidator"/> class.
    /// </summary>
    public GetServicesByCategoryQueryValidator()
    {
        this.RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid service category");
    }
}