// <copyright file="ProjectMappingProfile.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Projects.Mapping;

using AutoMapper;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// AutoMapper profile for project mappings.
/// </summary>
public sealed class ProjectMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectMappingProfile"/> class.
    /// </summary>
    public ProjectMappingProfile()
    {
        this.CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.DurationMonths, opt => opt.MapFrom(src => src.DurationMonths))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.SectorId, opt => opt.MapFrom(src => src.SectorId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Technologies, opt => opt.MapFrom(src => src.Technologies.Select(t => t.Name).ToList()))
            .ForMember(dest => dest.ServiceIds, opt => opt.MapFrom(src => src.ServiceIds))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        this.CreateMap<Project, ProjectListItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.DurationMonths, opt => opt.MapFrom(src => src.DurationMonths))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.SectorId, opt => opt.MapFrom(src => src.SectorId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        this.CreateMap<CreateProjectCommand, Project>()
            .ConstructUsing(cmd => Project.Create(cmd.Name, cmd.Description, cmd.StartDate, cmd.DurationMonths, cmd.ClientId, cmd.SectorId));
    }
}