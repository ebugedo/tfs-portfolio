// <copyright file="ClientValidators.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Clients.Validators;

using FluentValidation;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Queries;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Validator for <see cref="CreateClientCommand"/>.
/// </summary>
public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateClientCommandValidator"/> class.
    /// </summary>
    public CreateClientCommandValidator()
    {
        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required")
            .MaximumLength(200).WithMessage("Client name must not exceed 200 characters");

        this.RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        this.RuleFor(x => x.LogoUrl)
            .Must(url => string.IsNullOrWhiteSpace(url) || TfsWebUrl.Create(url).IsValid).WithMessage("Invalid logo URL format")
            .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));
    }
}

/// <summary>
/// Validator for <see cref="UpdateClientCommand"/>.
/// </summary>
public sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateClientCommandValidator"/> class.
    /// </summary>
    public UpdateClientCommandValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Client ID is required");

        this.RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required")
            .MaximumLength(200).WithMessage("Client name must not exceed 200 characters");

        this.RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        this.RuleFor(x => x.LogoUrl)
            .Must(url => string.IsNullOrWhiteSpace(url) || TfsWebUrl.Create(url).IsValid).WithMessage("Invalid logo URL format")
            .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));
    }
}

/// <summary>
/// Validator for <see cref="GetClientByIdQuery"/>.
/// </summary>
public sealed class GetClientByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetClientByIdQueryValidator"/> class.
    /// </summary>
    public GetClientByIdQueryValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Client ID is required");
    }
}