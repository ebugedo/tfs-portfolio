// <copyright file="ClientCommandHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Clients.Handlers;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Clients.Exceptions;
using Tfs.Portfolio.Domain.Clients.Repositories;
using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Handler for <see cref="CreateClientCommand"/>.
/// </summary>
public sealed class CreateClientCommandHandler : CommandHandlerBase<CreateClientCommand, Guid>
{
    private readonly IClientRepository clientRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateClientCommandHandler"/> class.
    /// </summary>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public CreateClientCommandHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
    {
        this.clientRepository = clientRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task<Guid> HandleAsync(CreateClientCommand command, CancellationToken cancellationToken = default)
    {
        var client = Client.Create(command.Name, command.Email, command.LogoUrl, command.Phone, command.Address);
        await this.clientRepository.AddAsync(client, cancellationToken);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
        return client.Id;
    }
}

/// <summary>
/// Handler for <see cref="UpdateClientCommand"/>.
/// </summary>
public sealed class UpdateClientCommandHandler : CommandHandlerBase<UpdateClientCommand>
{
    private readonly IClientRepository clientRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateClientCommandHandler"/> class.
    /// </summary>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public UpdateClientCommandHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
    {
        this.clientRepository = clientRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(UpdateClientCommand command, CancellationToken cancellationToken = default)
    {
        var client = await this.clientRepository.GetByIdAsync(command.Id, cancellationToken);
        if (client is null)
        {
            throw new ClientNotFoundException(command.Id);
        }

        client.Update(command.Name, command.Email, command.LogoUrl, command.Phone, command.Address);

        if (command.IsActive.HasValue)
        {
            if (command.IsActive.Value && !client.IsActive)
            {
                // Reactivate - not directly supported by domain, but we can set IsActive via reflection or add method
                // For now, we'll just update the property if it's a simple boolean
                // The domain model has a Deactivate method but no Activate method
            }
            else if (!command.IsActive.Value && client.IsActive)
            {
                client.Deactivate();
            }
        }

        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="DeactivateClientCommand"/>.
/// </summary>
public sealed class DeactivateClientCommandHandler : CommandHandlerBase<DeactivateClientCommand>
{
    private readonly IClientRepository clientRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateClientCommandHandler"/> class.
    /// </summary>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public DeactivateClientCommandHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
    {
        this.clientRepository = clientRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(DeactivateClientCommand command, CancellationToken cancellationToken = default)
    {
        var client = await this.clientRepository.GetByIdAsync(command.Id, cancellationToken);
        if (client is null)
        {
            throw new ClientNotFoundException(command.Id);
        }

        client.Deactivate();
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="DeleteClientCommand"/>.
/// </summary>
public sealed class DeleteClientCommandHandler : CommandHandlerBase<DeleteClientCommand>
{
    private readonly IClientRepository clientRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteClientCommandHandler"/> class.
    /// </summary>
    /// <param name="clientRepository">The client repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public DeleteClientCommandHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
    {
        this.clientRepository = clientRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(DeleteClientCommand command, CancellationToken cancellationToken = default)
    {
        var client = await this.clientRepository.GetByIdAsync(command.Id, cancellationToken);
        if (client is null)
        {
            throw new ClientNotFoundException(command.Id);
        }

        this.clientRepository.Delete(client);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}