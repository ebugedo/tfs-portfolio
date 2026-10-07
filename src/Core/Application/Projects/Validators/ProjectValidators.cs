// <copyright file="ProjectValidators.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Validators;

using FluentValidation;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Queries;
using Tfs.Portfolio.Domain.Projects.Entities;

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

        this.RuleFor(x => x.DurationMonths)
            .InclusiveBetween(1, 120)
            .WithMessage("Duration must be between 1 and 120 months");

        this.RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");

        this.RuleFor(x => x.SectorId)
            .NotEmpty()
            .WithMessage("Sector ID is required");
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

        this.RuleFor(x => x.DurationMonths)
            .InclusiveBetween(1, 120)
            .WithMessage("Duration must be between 1 and 120 months");

        this.RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");

        this.RuleFor(x => x.SectorId)
            .NotEmpty()
            .WithMessage("Sector ID is required");
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
/// Validator for <see cref="ChangeProjectStatusCommand"/>.
/// </summary>
public sealed class ChangeProjectStatusCommandValidator : AbstractValidator<ChangeProjectStatusCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeProjectStatusCommandValidator"/> class.
    /// </summary>
    public ChangeProjectStatusCommandValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Project ID is required");

        this.RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid project status");
    }
}

/// <summary>
/// Validator for <see cref="AddProjectServiceCommand"/>.
/// </summary>
public sealed class AddProjectServiceCommandValidator : AbstractValidator<AddProjectServiceCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddProjectServiceCommandValidator"/> class.
    /// </summary>
    public AddProjectServiceCommandValidator()
    {
        this.RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required");

        this.RuleFor(x => x.ServiceId)
            .NotEmpty()
            .WithMessage("Service ID is required");
    }
}

/// <summary>
/// Validator for <see cref="RemoveProjectServiceCommand"/>.
/// </summary>
public sealed class RemoveProjectServiceCommandValidator : AbstractValidator<RemoveProjectServiceCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveProjectServiceCommandValidator"/> class.
    /// </summary>
    public RemoveProjectServiceCommandValidator()
    {
        this.RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required");

        this.RuleFor(x => x.ServiceId)
            .NotEmpty()
            .WithMessage("Service ID is required");
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

/// <summary>
/// Validator for <see cref="GetProjectsByClientQuery"/>.
/// </summary>
public sealed class GetProjectsByClientQueryValidator : AbstractValidator<GetProjectsByClientQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsByClientQueryValidator"/> class.
    /// </summary>
    public GetProjectsByClientQueryValidator()
    {
        this.RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");
    }
}

/// <summary>
/// Validator for <see cref="GetProjectsBySectorQuery"/>.
/// </summary>
public sealed class GetProjectsBySectorQueryValidator : AbstractValidator<GetProjectsBySectorQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsBySectorQueryValidator"/> class.
    /// </summary>
    public GetProjectsBySectorQueryValidator()
    {
        this.RuleFor(x => x.SectorId)
            .NotEmpty()
            .WithMessage("Sector ID is required");
    }
}

/// <summary>
/// Validator for <see cref="GetProjectsByTechnologyQuery"/>.
/// </summary>
public sealed class GetProjectsByTechnologyQueryValidator : AbstractValidator<GetProjectsByTechnologyQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsByTechnologyQueryValidator"/> class.
    /// </summary>
    public GetProjectsByTechnologyQueryValidator()
    {
        this.RuleFor(x => x.TechnologyName)
            .NotEmpty()
            .WithMessage("Technology name is required");
    }
}

/// <summary>
/// Validator for <see cref="GetProjectsByStatusQuery"/>.
/// </summary>
public sealed class GetProjectsByStatusQueryValidator : AbstractValidator<GetProjectsByStatusQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetProjectsByStatusQueryValidator"/> class.
    /// </summary>
    public GetProjectsByStatusQueryValidator()
    {
        this.RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid project status");
    }
}