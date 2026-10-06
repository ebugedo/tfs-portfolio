// <copyright file="ApplicationModule.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Common.Modules;

using Autofac;
using AutoMapper;
using FluentValidation;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Projects.Handlers;
using Tfs.Portfolio.Application.Projects.Mapping;
using Tfs.Portfolio.Application.Projects.Queries;
using Tfs.Portfolio.Application.Projects.Validators;

/// <summary>
/// Autofac module for the Application layer.
/// </summary>
public sealed class ApplicationModule : Module
{
    /// <inheritdoc />
protected override void Load(ContainerBuilder builder)
        {
            // Register AutoMapper
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProjectMappingProfile>();
            })).AsSelf().SingleInstance();

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper(ctx.Resolve))
                .As<IMapper>()
                .InstancePerLifetimeScope();

            // Register MediatR-like handlers - both as interfaces and concrete types for Minimal API DI
            builder.RegisterType<CreateProjectCommandHandler>()
                .As<ICommandHandler<CreateProjectCommand, Guid>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<UpdateProjectCommandHandler>()
                .As<ICommandHandler<UpdateProjectCommand>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<DeleteProjectCommandHandler>()
                .As<ICommandHandler<DeleteProjectCommand>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetProjectByIdQueryHandler>()
                .As<IQueryHandler<GetProjectByIdQuery, ProjectDto>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetProjectsQueryHandler>()
                .As<IQueryHandler<GetProjectsQuery, IReadOnlyList<ProjectListItemDto>>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            // Register FluentValidation validators
            builder.RegisterType<CreateProjectCommandValidator>()
                .As<IValidator<CreateProjectCommand>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UpdateProjectCommandValidator>()
                .As<IValidator<UpdateProjectCommand>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DeleteProjectCommandValidator>()
                .As<IValidator<DeleteProjectCommand>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetProjectByIdQueryValidator>()
                .As<IValidator<GetProjectByIdQuery>>()
                .InstancePerLifetimeScope();
        }
}