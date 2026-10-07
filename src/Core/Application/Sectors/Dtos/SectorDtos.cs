// <copyright file="SectorDtos.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Sectors.Dtos;

/// <summary>
/// Data transfer object for a sector.
/// </summary>
public sealed record SectorDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Data transfer object for a sector list item.
/// </summary>
public sealed record SectorListItemDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt
);

/// <summary>
/// Request to create a new sector.
/// </summary>
public sealed record CreateSectorRequest(
    string Name,
    string? Description
);

/// <summary>
/// Request to update an existing sector.
/// </summary>
public sealed record UpdateSectorRequest(
    string Name,
    string? Description
);