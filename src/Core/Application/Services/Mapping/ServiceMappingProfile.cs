// <copyright file="ServiceMappingProfile.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Services.Mapping;

using AutoMapper;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// AutoMapper profile for service mappings.
/// </summary>
public sealed class ServiceMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceMappingProfile"/> class.
    /// </summary>
    public ServiceMappingProfile()
    {
        this.CreateMap<Service, ServiceDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        this.CreateMap<Service, ServiceListItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        this.CreateMap<CreateServiceCommand, Service>()
            .ConstructUsing(cmd => Service.Create(cmd.Name, cmd.Description, cmd.Category));
    }
}