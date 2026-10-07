// <copyright file="SectorMappingProfile.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Sectors.Mapping;

using AutoMapper;
using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Application.Sectors.Dtos;
using Tfs.Portfolio.Domain.Sectors.Entities;

/// <summary>
/// AutoMapper profile for sector mappings.
/// </summary>
public sealed class SectorMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SectorMappingProfile"/> class.
    /// </summary>
    public SectorMappingProfile()
    {
        this.CreateMap<Sector, SectorDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        this.CreateMap<Sector, SectorListItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        this.CreateMap<CreateSectorCommand, Sector>()
            .ConstructUsing(cmd => Sector.Create(cmd.Name, cmd.Description));
    }
}