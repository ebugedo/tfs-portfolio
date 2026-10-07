// <copyright file="SectorCommandHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Sectors.Handlers;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Domain.Sectors.Entities;
using Tfs.Portfolio.Domain.Sectors.Exceptions;
using Tfs.Portfolio.Domain.Sectors.Repositories;
using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Handler for <see cref="CreateSectorCommand"/>.
/// </summary>
public sealed class CreateSectorCommandHandler : CommandHandlerBase<CreateSectorCommand, Guid>
{
    private readonly ISectorRepository sectorRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSectorCommandHandler"/> class.
    /// </summary>
    /// <param name="sectorRepository">The sector repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public CreateSectorCommandHandler(
        ISectorRepository sectorRepository,
        IUnitOfWork unitOfWork)
    {
        this.sectorRepository = sectorRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task<Guid> HandleAsync(CreateSectorCommand command, CancellationToken cancellationToken = default)
    {
        var sector = Sector.Create(command.Name, command.Description);
        await this.sectorRepository.AddAsync(sector, cancellationToken);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
        return sector.Id;
    }
}

/// <summary>
/// Handler for <see cref="UpdateSectorCommand"/>.
/// </summary>
public sealed class UpdateSectorCommandHandler : CommandHandlerBase<UpdateSectorCommand>
{
    private readonly ISectorRepository sectorRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSectorCommandHandler"/> class.
    /// </summary>
    /// <param name="sectorRepository">The sector repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public UpdateSectorCommandHandler(
        ISectorRepository sectorRepository,
        IUnitOfWork unitOfWork)
    {
        this.sectorRepository = sectorRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(UpdateSectorCommand command, CancellationToken cancellationToken = default)
    {
        var sector = await this.sectorRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sector is null)
        {
            throw new SectorNotFoundException(command.Id);
        }

        sector.Update(command.Name, command.Description);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Handler for <see cref="DeleteSectorCommand"/>.
/// </summary>
public sealed class DeleteSectorCommandHandler : CommandHandlerBase<DeleteSectorCommand>
{
    private readonly ISectorRepository sectorRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSectorCommandHandler"/> class.
    /// </summary>
    /// <param name="sectorRepository">The sector repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public DeleteSectorCommandHandler(
        ISectorRepository sectorRepository,
        IUnitOfWork unitOfWork)
    {
        this.sectorRepository = sectorRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(DeleteSectorCommand command, CancellationToken cancellationToken = default)
    {
        var sector = await this.sectorRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sector is null)
        {
            throw new SectorNotFoundException(command.Id);
        }

        this.sectorRepository.Delete(sector);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}