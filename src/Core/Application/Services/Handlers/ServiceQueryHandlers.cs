// <copyright file="ServiceQueryHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Services.Handlers;

using AutoMapper;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Application.Services.Queries;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.Domain.Services.Repositories;

/// <summary>
/// Handler for <see cref="GetServiceByIdQuery"/>.
/// </summary>
public sealed class GetServiceByIdQueryHandler : QueryHandlerBase<GetServiceByIdQuery, ServiceDto>
{
    private readonly IServiceRepository serviceRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetServiceByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetServiceByIdQueryHandler(
        IServiceRepository serviceRepository,
        IMapper mapper)
    {
        this.serviceRepository = serviceRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<ServiceDto> HandleAsync(GetServiceByIdQuery query, CancellationToken cancellationToken = default)
    {
        var service = await this.serviceRepository.GetByIdAsync(query.Id, cancellationToken);
        if (service is null)
        {
            return null!;
        }
        return this.mapper.Map<ServiceDto>(service);
    }
}

/// <summary>
/// Handler for <see cref="GetServicesQuery"/>.
/// </summary>
public sealed class GetServicesQueryHandler : QueryHandlerBase<GetServicesQuery, IReadOnlyList<ServiceListItemDto>>
{
    private readonly IServiceRepository serviceRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetServicesQueryHandler"/> class.
    /// </summary>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetServicesQueryHandler(
        IServiceRepository serviceRepository,
        IMapper mapper)
    {
        this.serviceRepository = serviceRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<ServiceListItemDto>> HandleAsync(GetServicesQuery query, CancellationToken cancellationToken = default)
    {
        var services = await this.serviceRepository.GetAllAsync(cancellationToken);
        return this.mapper.Map<IReadOnlyList<ServiceListItemDto>>(services);
    }
}

/// <summary>
/// Handler for <see cref="GetActiveServicesQuery"/>.
/// </summary>
public sealed class GetActiveServicesQueryHandler : QueryHandlerBase<GetActiveServicesQuery, IReadOnlyList<ServiceListItemDto>>
{
    private readonly IServiceRepository serviceRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetActiveServicesQueryHandler"/> class.
    /// </summary>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetActiveServicesQueryHandler(
        IServiceRepository serviceRepository,
        IMapper mapper)
    {
        this.serviceRepository = serviceRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<ServiceListItemDto>> HandleAsync(GetActiveServicesQuery query, CancellationToken cancellationToken = default)
    {
        var services = await this.serviceRepository.GetActiveAsync(cancellationToken);
        return this.mapper.Map<IReadOnlyList<ServiceListItemDto>>(services);
    }
}

/// <summary>
/// Handler for <see cref="GetServicesByCategoryQuery"/>.
/// </summary>
public sealed class GetServicesByCategoryQueryHandler : QueryHandlerBase<GetServicesByCategoryQuery, IReadOnlyList<ServiceListItemDto>>
{
    private readonly IServiceRepository serviceRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetServicesByCategoryQueryHandler"/> class.
    /// </summary>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetServicesByCategoryQueryHandler(
        IServiceRepository serviceRepository,
        IMapper mapper)
    {
        this.serviceRepository = serviceRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<ServiceListItemDto>> HandleAsync(GetServicesByCategoryQuery query, CancellationToken cancellationToken = default)
    {
        var services = await this.serviceRepository.GetByCategoryAsync(query.Category, cancellationToken);
        return this.mapper.Map<IReadOnlyList<ServiceListItemDto>>(services);
    }
}