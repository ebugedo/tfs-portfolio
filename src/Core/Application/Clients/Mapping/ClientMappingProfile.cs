// <copyright file="ClientMappingProfile.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Clients.Mapping;

using AutoMapper;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// AutoMapper profile for client mappings.
/// </summary>
public sealed class ClientMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientMappingProfile"/> class.
    /// </summary>
    public ClientMappingProfile()
    {
        this.CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        this.CreateMap<Client, ClientListItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        this.CreateMap<CreateClientCommand, Client>()
            .ConstructUsing(cmd => Client.Create(cmd.Name, cmd.Email, cmd.LogoUrl, cmd.Phone, cmd.Address));
    }
}