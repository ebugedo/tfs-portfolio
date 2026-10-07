// <copyright file="ClientQueries.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Clients.Queries;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Clients.Dtos;

/// <summary>
/// Query to get a client by its identifier.
/// </summary>
public sealed record GetClientByIdQuery(
    Guid Id
) : IQuery<ClientDto>;

/// <summary>
/// Query to get all clients.
/// </summary>
public sealed record GetClientsQuery
    : IQuery<IReadOnlyList<ClientListItemDto>>;

/// <summary>
/// Query to get all active clients.
/// </summary>
public sealed record GetActiveClientsQuery
    : IQuery<IReadOnlyList<ClientListItemDto>>;