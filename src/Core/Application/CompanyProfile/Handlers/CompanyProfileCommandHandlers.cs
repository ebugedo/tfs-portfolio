// <copyright file="CompanyProfileCommandHandlers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.CompanyProfile.Handlers;

using Tfs.Portfolio.Application.Common.CQRS;
using Tfs.Portfolio.Application.CompanyProfile.Commands;
using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Domain.CompanyProfile.Exceptions;
using Tfs.Portfolio.Domain.CompanyProfile.Repositories;
using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Handler for <see cref="UpdateCompanyProfileCommand"/>.
/// </summary>
public sealed class UpdateCompanyProfileCommandHandler : CommandHandlerBase<UpdateCompanyProfileCommand>
{
    private readonly ICompanyProfileRepository companyProfileRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCompanyProfileCommandHandler"/> class.
    /// </summary>
    /// <param name="companyProfileRepository">The company profile repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public UpdateCompanyProfileCommandHandler(
        ICompanyProfileRepository companyProfileRepository,
        IUnitOfWork unitOfWork)
    {
        this.companyProfileRepository = companyProfileRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(UpdateCompanyProfileCommand command, CancellationToken cancellationToken = default)
    {
        var profile = await this.companyProfileRepository.GetAsync(cancellationToken);
        if (profile is null)
        {
            throw new CompanyProfileNotFoundException();
        }

        profile.Update(command.CompanyName, command.ContactEmail, command.ContactPhone, command.TfsWebUrl, command.Address, command.Description, command.LogoUrl);
        await this.unitOfWork.SaveChangesAsync(cancellationToken);
    }
}