// <copyright file="ProjectValidators.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Validators;

using FluentValidation;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Queries;

/// <summary>
/// Validator for <see cref="CreateProjectCommand"/>.
/// </summary>
public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectCommandValidator"/> class.
    /// </summary>
    public CreateProjectCommandValidator()
    {
        this.RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .MaximumLength(200)
            .WithMessage("Project name must not exceed 200 characters");

        this.RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Project description must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

/// <summary>
/// Validator for <see cref="UpdateProjectCommand"/>.
/// </summary>
public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProjectCommandValidator"/> class.
    /// </summary>
    public UpdateProjectCommandValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Project ID is required");

        this.RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .MaximumLength(200)
            .WithMessage("Project name must not exceed 200 characters");

        this.RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Project description must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

/// <summary>
/// Validator for <see cref="DeleteProjectCommand"/>.
/// </summary>
public sealed class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProjectCommandValidator"/> class.
    /// </summary>
    public DeleteProjectCommandValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Project ID is required");
    }
}

/// <summary>
/// Validator for <see cref="GetProjectByIdQuery"/>.
/// </summary>
public sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectByIdQueryValidator"/> class.
    /// </summary>
    public GetProjectByIdQueryValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Project ID is required");
    }
}