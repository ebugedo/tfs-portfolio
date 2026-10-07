// <copyright file="ServiceQueries.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Services.Queries;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// Query to get a service by its identifier.
/// </summary>
public sealed record GetServiceByIdQuery(
    Guid Id
) : IQuery<ServiceDto>;

/// <summary>
/// Query to get all services.
/// </summary>
public sealed record GetServicesQuery
    : IQuery<IReadOnlyList<ServiceListItemDto>>;

/// <summary>
/// Query to get all active services.
/// </summary>
public sealed record GetActiveServicesQuery
    : IQuery<IReadOnlyList<ServiceListItemDto>>;

/// <summary>
/// Query to get services by category.
/// </summary>
public sealed record GetServicesByCategoryQuery(
    ServiceCategory Category
) : IQuery<IReadOnlyList<ServiceListItemDto>>;