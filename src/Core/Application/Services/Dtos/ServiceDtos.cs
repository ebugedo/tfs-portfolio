// <copyright file="ServiceDtos.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Services.Dtos;

using Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// Data transfer object for a service.
/// </summary>
public sealed record ServiceDto(
    Guid Id,
    string Name,
    string? Description,
    ServiceCategory Category,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Data transfer object for a service list item.
/// </summary>
public sealed record ServiceListItemDto(
    Guid Id,
    string Name,
    string? Description,
    ServiceCategory Category,
    bool IsActive,
    DateTime CreatedAt
);

/// <summary>
/// Request to create a new service.
/// </summary>
public sealed record CreateServiceRequest(
    string Name,
    string? Description,
    ServiceCategory Category
);

/// <summary>
/// Request to update an existing service.
/// </summary>
public sealed record UpdateServiceRequest(
    string Name,
    string? Description,
    ServiceCategory Category
);