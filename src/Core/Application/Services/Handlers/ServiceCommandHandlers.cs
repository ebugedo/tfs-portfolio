// <copyright file="ServiceCommandHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Services.Handlers;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.Domain.Services.Exceptions;
using Tfs.Portfolio.Domain.Services.Repositories;
using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Handler for <see cref="CreateServiceCommand"/>.
/// </summary>
public sealed class CreateServiceCommandHandler : CommandHandlerBase<CreateServiceCommand, Guid>
{
    private readonly IServiceRepository serviceRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateServiceCommandHandler"/> class.
    /// </summary>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public CreateServiceCommandHandler(
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        this.serviceRepository = serviceRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task<Guid> HandleAsync(CreateServiceCommand command, CancellationToken cancellationToken = default)
    {
        var service = Service.Create(command.Name, command.Description, command.Category);
        await this.serviceRepository.AddAsync(service, cancellationToken);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
        return service.Id;
    }
}

/// <summary>
/// Handler for <see cref="UpdateServiceCommand"/>.
/// </summary>
public sealed class UpdateServiceCommandHandler : CommandHandlerBase<UpdateServiceCommand>
{
    private readonly IServiceRepository serviceRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateServiceCommandHandler"/> class.
    /// </summary>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public UpdateServiceCommandHandler(
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        this.serviceRepository = serviceRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(UpdateServiceCommand command, CancellationToken cancellationToken = default)
    {
        var service = await this.serviceRepository.GetByIdAsync(command.Id, cancellationToken);
        if (service is null)
        {
            throw new ServiceNotFoundException(command.Id);
        }

        service.Update(command.Name, command.Description, command.Category);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="DeleteServiceCommand"/>.
/// </summary>
public sealed class DeleteServiceCommandHandler : CommandHandlerBase<DeleteServiceCommand>
{
    private readonly IServiceRepository serviceRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteServiceCommandHandler"/> class.
    /// </summary>
    /// <param name="serviceRepository">The service repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public DeleteServiceCommandHandler(
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        this.serviceRepository = serviceRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(DeleteServiceCommand command, CancellationToken cancellationToken = default)
    {
        var service = await this.serviceRepository.GetByIdAsync(command.Id, cancellationToken);
        if (service is null)
        {
            throw new ServiceNotFoundException(command.Id);
        }

        this.serviceRepository.Delete(service);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}