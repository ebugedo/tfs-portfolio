// <copyright file="SectorQueries.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Sectors.Queries;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Sectors.Dtos;

/// <summary>
/// Query to get a sector by its identifier.
/// </summary>
public sealed record GetSectorByIdQuery(
    Guid Id
) : IQuery<SectorDto>;

/// <summary>
/// Query to get all sectors.
/// </summary>
public sealed record GetSectorsQuery
    : IQuery<IReadOnlyList<SectorListItemDto>>;

/// <summary>
/// Query to get all active sectors.
/// </summary>
public sealed record GetActiveSectorsQuery
    : IQuery<IReadOnlyList<SectorListItemDto>>;