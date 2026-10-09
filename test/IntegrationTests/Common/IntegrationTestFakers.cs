// <copyright file="IntegrationTestFakers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Common;

using Bogus;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Application.Sectors.Commands;
using Tfs.Portfolio.Application.Sectors.Dtos;
using Tfs.Portfolio.Application.Services.Commands;
using Tfs.Portfolio.Application.Services.Dtos;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Sectors.Entities;
using Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// Bogus fakers for generating test data in integration tests.
/// </summary>
public static class IntegrationTestFakers
{
    private static readonly Faker<CreateClientCommand> CreateClientCommandFaker = new Faker<CreateClientCommand>()
        .CustomInstantiator(f => new CreateClientCommand(
            f.Company.CompanyName(),
            f.Internet.Email(),
            TfsWebUrl.Create($"https://{f.Internet.DomainName()}/logo.png"),
            f.Phone.PhoneNumber(),
            f.Address.FullAddress()));

    private static readonly Faker<CreateSectorCommand> CreateSectorCommandFaker = new Faker<CreateSectorCommand>()
        .CustomInstantiator(f => new CreateSectorCommand(
            f.Commerce.Categories(1)[0],
            f.Lorem.Sentence()));

    private static readonly Faker<CreateServiceCommand> CreateServiceCommandFaker = new Faker<CreateServiceCommand>()
        .CustomInstantiator(f => new CreateServiceCommand(
            f.Commerce.ProductName(),
            f.Lorem.Sentence(),
            f.PickRandom<ServiceCategory>()));

    private static readonly Faker<CreateProjectCommand> CreateProjectCommandFaker = new Faker<CreateProjectCommand>()
        .CustomInstantiator(f => new CreateProjectCommand(
            f.Commerce.ProductName(),
            f.Lorem.Paragraph(),
            YearMonth.Create(f.Random.Int(1, 12), f.Date.Past(5).Year),
            f.Random.Int(1, 120),
            Guid.NewGuid(), // Will be replaced by test
            Guid.NewGuid(), // Will be replaced by test
            new List<string> { "C#", ".NET", "Azure" }));

    /// <summary>
    /// Generates a valid CreateClientCommand.
    /// </summary>
    public static CreateClientCommand GenerateCreateClientCommand()
    {
        return CreateClientCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid CreateSectorCommand.
    /// </summary>
    public static CreateSectorCommand GenerateCreateSectorCommand()
    {
        return CreateSectorCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid CreateServiceCommand.
    /// </summary>
    public static CreateServiceCommand GenerateCreateServiceCommand()
    {
        return CreateServiceCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid CreateProjectCommand with specified client and sector IDs.
    /// </summary>
    public static CreateProjectCommand GenerateCreateProjectCommand(Guid clientId, Guid sectorId)
    {
        var command = CreateProjectCommandFaker.Generate();
        return command with { ClientId = clientId, SectorId = sectorId };
    }
}