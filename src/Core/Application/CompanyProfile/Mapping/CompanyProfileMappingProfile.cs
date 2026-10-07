// <copyright file="CompanyProfileMappingProfile.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.CompanyProfile.Mapping;

using AutoMapper;
using Tfs.Portfolio.Application.CompanyProfile.Commands;
using Tfs.Portfolio.Application.CompanyProfile.Dtos;
using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// AutoMapper profile for company profile mappings.
/// </summary>
public sealed class CompanyProfileMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyProfileMappingProfile"/> class.
    /// </summary>
    public CompanyProfileMappingProfile()
    {
        this.CreateMap<CompanyProfileEntity, CompanyProfileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
            .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.ContactEmail))
            .ForMember(dest => dest.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
            .ForMember(dest => dest.WebUrl, opt => opt.MapFrom(src => src.WebUrl))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }
}