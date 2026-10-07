// <copyright file="ClientQueryHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Clients.Handlers;

using AutoMapper;
using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Application.Clients.Queries;
using Tfs.Portfolio.Domain.Clients.Repositories;

/// <summary>
/// Handler for <see cref="GetClientByIdQuery"/>.
/// </summary>
public sealed class GetClientByIdQueryHandler : QueryHandlerBase<GetClientByIdQuery, ClientDto>
{
    private readonly IClientRepository clientRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetClientByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetClientByIdQueryHandler(
        IClientRepository clientRepository,
        IMapper mapper)
    {
        this.clientRepository = clientRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<ClientDto> HandleAsync(GetClientByIdQuery query, CancellationToken cancellationToken = default)
    {
        var client = await this.clientRepository.GetByIdAsync(query.Id, cancellationToken);
        if (client is null)
        {
            return null!;
        }
        return this.mapper.Map<ClientDto>(client);
    }
}

/// <summary>
/// Handler for <see cref="GetClientsQuery"/>.
/// </summary>
public sealed class GetClientsQueryHandler : QueryHandlerBase<GetClientsQuery, IReadOnlyList<ClientListItemDto>>
{
    private readonly IClientRepository clientRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetClientsQueryHandler"/> class.
    /// </summary>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetClientsQueryHandler(
        IClientRepository clientRepository,
        IMapper mapper)
    {
        this.clientRepository = clientRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<ClientListItemDto>> HandleAsync(GetClientsQuery query, CancellationToken cancellationToken = default)
    {
        var clients = await this.clientRepository.GetAllAsync(cancellationToken);
        return this.mapper.Map<IReadOnlyList<ClientListItemDto>>(clients);
    }
}

/// <summary>
/// Handler for <see cref="GetActiveClientsQuery"/>.
/// </summary>
public sealed class GetActiveClientsQueryHandler : QueryHandlerBase<GetActiveClientsQuery, IReadOnlyList<ClientListItemDto>>
{
    private readonly IClientRepository clientRepository;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetActiveClientsQueryHandler"/> class.
    /// </summary>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetActiveClientsQueryHandler(
        IClientRepository clientRepository,
        IMapper mapper)
    {
        this.clientRepository = clientRepository;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public override async Task<IReadOnlyList<ClientListItemDto>> HandleAsync(GetActiveClientsQuery query, CancellationToken cancellationToken = default)
    {
        var clients = await this.clientRepository.GetActiveAsync(cancellationToken);
        return this.mapper.Map<IReadOnlyList<ClientListItemDto>>(clients);
    }
}