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
            .ConstructUsing(src => new ClientDto(
                src.Id,
                src.Name,
                src.Email,
                src.LogoUrl,
                src.Phone,
                src.Address,
                src.IsActive,
                src.CreatedAt,
                src.UpdatedAt))
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl));

        this.CreateMap<Client, ClientListItemDto>()
            .ConstructUsing(src => new ClientListItemDto(
                src.Id,
                src.Name,
                src.Email,
                src.LogoUrl,
                src.IsActive,
                src.CreatedAt))
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl));

        this.CreateMap<CreateClientCommand, Client>()
            .ConstructUsing(cmd => Client.Create(cmd.Name, cmd.Email, cmd.LogoUrl, cmd.Phone, cmd.Address));
    }
}