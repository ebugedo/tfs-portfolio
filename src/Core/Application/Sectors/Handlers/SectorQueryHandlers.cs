// <copyright file="SectorQueryHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Sectors.Handlers;

using AutoMapper;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Sectors.Dtos;
using Tfs.Portfolio.Application.Sectors.Queries;
using Tfs.Portfolio.Domain.Sectors.Repositories;

/// <summary>
/// Handler for <see cref="GetSectorByIdQuery"/>.
/// </summary>
public sealed class GetSectorByIdQueryHandler : QueryHandlerBase<GetSectorByIdQuery, SectorDto>
{
    private readonly ISectorRepository sectorRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSectorByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="sectorRepository">The sector repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetSectorByIdQueryHandler(
        ISectorRepository sectorRepository,
        IMapper mapper)
    {
        this.sectorRepository = sectorRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<SectorDto> HandleAsync(GetSectorByIdQuery query, CancellationToken cancellationToken = default)
    {
        var sector = await this.sectorRepository.GetByIdAsync(query.Id, cancellationToken);
        if (sector is null)
        {
            return null!;
        }
        return this.mapper.Map<SectorDto>(sector);
    }
}

/// <summary>
/// Handler for <see cref="GetSectorsQuery"/>.
/// </summary>
public sealed class GetSectorsQueryHandler : QueryHandlerBase<GetSectorsQuery, IReadOnlyList<SectorListItemDto>>
{
    private readonly ISectorRepository sectorRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSectorsQueryHandler"/> class.
    /// </summary>
    /// <param name="sectorRepository">The sector repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetSectorsQueryHandler(
        ISectorRepository sectorRepository,
        IMapper mapper)
    {
        this.sectorRepository = sectorRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<SectorListItemDto>> HandleAsync(GetSectorsQuery query, CancellationToken cancellationToken = default)
    {
        var sectors = await this.sectorRepository.GetAllAsync(cancellationToken);
        return this.mapper.Map<IReadOnlyList<SectorListItemDto>>(sectors);
    }
}

/// <summary>
/// Handler for <see cref="GetActiveSectorsQuery"/>.
/// </summary>
public sealed class GetActiveSectorsQueryHandler : QueryHandlerBase<GetActiveSectorsQuery, IReadOnlyList<SectorListItemDto>>
{
    private readonly ISectorRepository sectorRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetActiveSectorsQueryHandler"/> class.
    /// </summary>
    /// <param name="sectorRepository">The sector repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetActiveSectorsQueryHandler(
        ISectorRepository sectorRepository,
        IMapper mapper)
    {
        this.sectorRepository = sectorRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<SectorListItemDto>> HandleAsync(GetActiveSectorsQuery query, CancellationToken cancellationToken = default)
    {
        var sectors = await this.sectorRepository.GetActiveAsync(cancellationToken);
        return this.mapper.Map<IReadOnlyList<SectorListItemDto>>(sectors);
    }
}