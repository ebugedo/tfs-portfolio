// <copyright file="ApplicationModule.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Common.Modules;

using Autofac;
using AutoMapper;
using FluentValidation;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Application.Clients.Handlers;
using Tfs.Portfolio.Application.Clients.Mapping;
using Tfs.Portfolio.Application.Clients.Queries;
using Tfs.Portfolio.Application.Clients.Validators;
using Tfs.Portfolio.Application.CompanyProfile.Commands;
using Tfs.Portfolio.Application.CompanyProfile.Dtos;
using Tfs.Portfolio.Application.CompanyProfile.Handlers;
using Tfs.Portfolio.Application.CompanyProfile.Mapping;
using Tfs.Portfolio.Application.CompanyProfile.Queries;
using Tfs.Portfolio.Application.CompanyProfile.Validators;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Projects.Handlers;
using Tfs.Portfolio.Application.Projects.Mapping;
using Tfs.Portfolio.Application.Projects.Queries;
using Tfs.Portfolio.Application.Projects.Validators;
using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Application.Sectors.Dtos;
using Tfs.Portfolio.Application.Sectors.Handlers;
using Tfs.Portfolio.Application.Sectors.Mapping;
using Tfs.Portfolio.Application.Sectors.Queries;
using Tfs.Portfolio.Application.Sectors.Validators;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Application.Services.Handlers;
using Tfs.Portfolio.Application.Services.Mapping;
using Tfs.Portfolio.Application.Services.Queries;
using Tfs.Portfolio.Application.Services.Validators;

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
            cfg.AddProfile<ClientMappingProfile>();
            cfg.AddProfile<SectorMappingProfile>();
            cfg.AddProfile<ServiceMappingProfile>();
            cfg.AddProfile<CompanyProfileMappingProfile>();
        })).AsSelf().SingleInstance();

        builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper(ctx.Resolve))
            .As<IMapper>()
            .InstancePerLifetimeScope();

        // Register Project handlers
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

        builder.RegisterType<ChangeProjectStatusCommandHandler>()
            .As<ICommandHandler<ChangeProjectStatusCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<AddProjectServiceCommandHandler>()
            .As<ICommandHandler<AddProjectServiceCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<RemoveProjectServiceCommandHandler>()
            .As<ICommandHandler<RemoveProjectServiceCommand>>()
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

        builder.RegisterType<GetProjectsByClientQueryHandler>()
            .As<IQueryHandler<GetProjectsByClientQuery, IReadOnlyList<ProjectListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetProjectsBySectorQueryHandler>()
            .As<IQueryHandler<GetProjectsBySectorQuery, IReadOnlyList<ProjectListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetProjectsByTechnologyQueryHandler>()
            .As<IQueryHandler<GetProjectsByTechnologyQuery, IReadOnlyList<ProjectListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetProjectsByStatusQueryHandler>()
            .As<IQueryHandler<GetProjectsByStatusQuery, IReadOnlyList<ProjectListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Register Client handlers
        builder.RegisterType<CreateClientCommandHandler>()
            .As<ICommandHandler<CreateClientCommand, Guid>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<UpdateClientCommandHandler>()
            .As<ICommandHandler<UpdateClientCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<DeleteClientCommandHandler>()
            .As<ICommandHandler<DeleteClientCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetClientByIdQueryHandler>()
            .As<IQueryHandler<GetClientByIdQuery, ClientDto>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetClientsQueryHandler>()
            .As<IQueryHandler<GetClientsQuery, IReadOnlyList<ClientListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetActiveClientsQueryHandler>()
            .As<IQueryHandler<GetActiveClientsQuery, IReadOnlyList<ClientListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Register Sector handlers
        builder.RegisterType<CreateSectorCommandHandler>()
            .As<ICommandHandler<CreateSectorCommand, Guid>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<UpdateSectorCommandHandler>()
            .As<ICommandHandler<UpdateSectorCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<DeleteSectorCommandHandler>()
            .As<ICommandHandler<DeleteSectorCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetSectorByIdQueryHandler>()
            .As<IQueryHandler<GetSectorByIdQuery, SectorDto>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetSectorsQueryHandler>()
            .As<IQueryHandler<GetSectorsQuery, IReadOnlyList<SectorListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetActiveSectorsQueryHandler>()
            .As<IQueryHandler<GetActiveSectorsQuery, IReadOnlyList<SectorListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Register Service handlers
        builder.RegisterType<CreateServiceCommandHandler>()
            .As<ICommandHandler<CreateServiceCommand, Guid>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<UpdateServiceCommandHandler>()
            .As<ICommandHandler<UpdateServiceCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<DeleteServiceCommandHandler>()
            .As<ICommandHandler<DeleteServiceCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetServiceByIdQueryHandler>()
            .As<IQueryHandler<GetServiceByIdQuery, ServiceDto>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetServicesQueryHandler>()
            .As<IQueryHandler<GetServicesQuery, IReadOnlyList<ServiceListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetActiveServicesQueryHandler>()
            .As<IQueryHandler<GetActiveServicesQuery, IReadOnlyList<ServiceListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetServicesByCategoryQueryHandler>()
            .As<IQueryHandler<GetServicesByCategoryQuery, IReadOnlyList<ServiceListItemDto>>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Register CompanyProfile handlers
        builder.RegisterType<UpdateCompanyProfileCommandHandler>()
            .As<ICommandHandler<UpdateCompanyProfileCommand>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetCompanyProfileQueryHandler>()
            .As<IQueryHandler<GetCompanyProfileQuery, CompanyProfileDto>>()
            .AsSelf()
            .InstancePerLifetimeScope();

        // Register FluentValidation validators - Project
        builder.RegisterType<CreateProjectCommandValidator>()
            .As<IValidator<CreateProjectCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<UpdateProjectCommandValidator>()
            .As<IValidator<UpdateProjectCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<DeleteProjectCommandValidator>()
            .As<IValidator<DeleteProjectCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<ChangeProjectStatusCommandValidator>()
            .As<IValidator<ChangeProjectStatusCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<AddProjectServiceCommandValidator>()
            .As<IValidator<AddProjectServiceCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<RemoveProjectServiceCommandValidator>()
            .As<IValidator<RemoveProjectServiceCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetProjectByIdQueryValidator>()
            .As<IValidator<GetProjectByIdQuery>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetProjectsByClientQueryValidator>()
            .As<IValidator<GetProjectsByClientQuery>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetProjectsBySectorQueryValidator>()
            .As<IValidator<GetProjectsBySectorQuery>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetProjectsByTechnologyQueryValidator>()
            .As<IValidator<GetProjectsByTechnologyQuery>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetProjectsByStatusQueryValidator>()
            .As<IValidator<GetProjectsByStatusQuery>>()
            .InstancePerLifetimeScope();

        // Register FluentValidation validators - Client
        builder.RegisterType<CreateClientCommandValidator>()
            .As<IValidator<CreateClientCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<UpdateClientCommandValidator>()
            .As<IValidator<UpdateClientCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetClientByIdQueryValidator>()
            .As<IValidator<GetClientByIdQuery>>()
            .InstancePerLifetimeScope();

        // Register FluentValidation validators - Sector
        builder.RegisterType<CreateSectorCommandValidator>()
            .As<IValidator<CreateSectorCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<UpdateSectorCommandValidator>()
            .As<IValidator<UpdateSectorCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetSectorByIdQueryValidator>()
            .As<IValidator<GetSectorByIdQuery>>()
            .InstancePerLifetimeScope();

        // Register FluentValidation validators - Service
        builder.RegisterType<CreateServiceCommandValidator>()
            .As<IValidator<CreateServiceCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<UpdateServiceCommandValidator>()
            .As<IValidator<UpdateServiceCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetServiceByIdQueryValidator>()
            .As<IValidator<GetServiceByIdQuery>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetServicesByCategoryQueryValidator>()
            .As<IValidator<GetServicesByCategoryQuery>>()
            .InstancePerLifetimeScope();

        // Register FluentValidation validators - CompanyProfile
        builder.RegisterType<UpdateCompanyProfileCommandValidator>()
            .As<IValidator<UpdateCompanyProfileCommand>>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GetCompanyProfileQueryValidator>()
            .As<IValidator<GetCompanyProfileQuery>>()
            .InstancePerLifetimeScope();
    }
}