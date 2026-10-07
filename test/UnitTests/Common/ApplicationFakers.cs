// <copyright file="ApplicationFakers.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Common;

using Bogus;
using Tfs.Portfolio.Application.Clients.Commands;
using Tfs.Portfolio.Application.Clients.Dtos;
using Tfs.Portfolio.Application.Projects.Commands;
using Tfs.Portfolio.Application.Projects.Dtos;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Factory for creating test commands and DTOs.
/// </summary>
public static class ApplicationFakers
{
    private static readonly Faker<CreateProjectCommand> CreateProjectCommandFaker = new Faker<CreateProjectCommand>()
        .CustomInstantiator(f => new CreateProjectCommand(
            f.Commerce.ProductName(),
            f.Lorem.Paragraph(),
            YearMonth.Create(f.Random.Int(1, 12), f.Date.Past(5).Year),
            f.Random.Int(1, 120),
            Guid.NewGuid(),
            Guid.NewGuid()));

    private static readonly Faker<UpdateProjectCommand> UpdateProjectCommandFaker = new Faker<UpdateProjectCommand>()
        .CustomInstantiator(f => new UpdateProjectCommand(
            f.Random.Guid(),
            f.Commerce.ProductName(),
            f.Lorem.Paragraph(),
            YearMonth.Create(f.Random.Int(1, 12), f.Date.Past(5).Year),
            f.Random.Int(1, 120),
            Guid.NewGuid(),
            Guid.NewGuid()));

    private static readonly Faker<DeleteProjectCommand> DeleteProjectCommandFaker = new Faker<DeleteProjectCommand>()
        .CustomInstantiator(f => new DeleteProjectCommand(f.Random.Guid()));

    private static readonly Faker<ProjectDto> ProjectDtoFaker = new Faker<ProjectDto>()
        .CustomInstantiator(f => new ProjectDto(
            f.Random.Guid(),
            f.Commerce.ProductName(),
            f.Lorem.Paragraph(),
            YearMonth.Create(f.Random.Int(1, 12), f.Date.Past(5).Year),
            f.Random.Int(1, 120),
            YearMonth.Create(f.Random.Int(1, 12), f.Date.Future(5).Year),
            Guid.NewGuid(),
            Guid.NewGuid(),
            ProjectStatus.Draft,
            [],
            [],
            f.Date.Recent(),
            f.Date.Recent()));

    private static readonly Faker<ProjectListItemDto> ProjectListItemDtoFaker = new Faker<ProjectListItemDto>()
        .CustomInstantiator(f => new ProjectListItemDto(
            f.Random.Guid(),
            f.Commerce.ProductName(),
            f.Lorem.Paragraph(),
            YearMonth.Create(f.Random.Int(1, 12), f.Date.Past(5).Year),
            f.Random.Int(1, 120),
            YearMonth.Create(f.Random.Int(1, 12), f.Date.Future(5).Year),
            Guid.NewGuid(),
            Guid.NewGuid(),
            ProjectStatus.Draft,
            f.Date.Recent()));

    // Client fakers
    private static readonly Faker<CreateClientCommand> CreateClientCommandFaker = new Faker<CreateClientCommand>()
        .CustomInstantiator(f => new CreateClientCommand(
            f.Company.CompanyName(),
            f.Internet.Email(),
            Url.Create($"https://{f.Internet.DomainName()}/logo.png"),
            f.Phone.PhoneNumber(),
            f.Address.FullAddress()));

    private static readonly Faker<UpdateClientCommand> UpdateClientCommandFaker = new Faker<UpdateClientCommand>()
        .CustomInstantiator(f => new UpdateClientCommand(
            f.Random.Guid(),
            f.Company.CompanyName(),
            f.Internet.Email(),
            Url.Create($"https://{f.Internet.DomainName()}/logo.png"),
            f.Phone.PhoneNumber(),
            f.Address.FullAddress(),
            f.Random.Bool()));

    private static readonly Faker<DeleteClientCommand> DeleteClientCommandFaker = new Faker<DeleteClientCommand>()
        .CustomInstantiator(f => new DeleteClientCommand(f.Random.Guid()));

    private static readonly Faker<ClientDto> ClientDtoFaker = new Faker<ClientDto>()
        .CustomInstantiator(f => new ClientDto(
            f.Random.Guid(),
            f.Company.CompanyName(),
            f.Internet.Email(),
            Url.Create($"https://{f.Internet.DomainName()}/logo.png"),
            f.Phone.PhoneNumber(),
            f.Address.FullAddress(),
            true,
            f.Date.Recent(),
            f.Date.Recent()));

    private static readonly Faker<ClientListItemDto> ClientListItemDtoFaker = new Faker<ClientListItemDto>()
        .CustomInstantiator(f => new ClientListItemDto(
            f.Random.Guid(),
            f.Company.CompanyName(),
            f.Internet.Email(),
            Url.Create($"https://{f.Internet.DomainName()}/logo.png"),
            true,
            f.Date.Recent()));

    /// <summary>
    /// Generates a valid CreateProjectCommand.
    /// </summary>
    public static CreateProjectCommand GenerateCreateProjectCommand()
    {
        return CreateProjectCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid UpdateProjectCommand.
    /// </summary>
    public static UpdateProjectCommand GenerateUpdateProjectCommand()
    {
        return UpdateProjectCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid DeleteProjectCommand.
    /// </summary>
    public static DeleteProjectCommand GenerateDeleteProjectCommand()
    {
        return DeleteProjectCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid ProjectDto.
    /// </summary>
    public static ProjectDto GenerateProjectDto()
    {
        return ProjectDtoFaker.Generate();
    }

    /// <summary>
    /// Generates a valid ProjectListItemDto.
    /// </summary>
    public static ProjectListItemDto GenerateProjectListItemDto()
    {
        return ProjectListItemDtoFaker.Generate();
    }

    /// <summary>
    /// Generates a valid CreateClientCommand.
    /// </summary>
    public static CreateClientCommand GenerateCreateClientCommand()
    {
        return CreateClientCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid UpdateClientCommand.
    /// </summary>
    public static UpdateClientCommand GenerateUpdateClientCommand()
    {
        return UpdateClientCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid DeleteClientCommand.
    /// </summary>
    public static DeleteClientCommand GenerateDeleteClientCommand()
    {
        return DeleteClientCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a valid ClientDto.
    /// </summary>
    public static ClientDto GenerateClientDto()
    {
        return ClientDtoFaker.Generate();
    }

    /// <summary>
    /// Generates a valid ClientListItemDto.
    /// </summary>
    public static ClientListItemDto GenerateClientListItemDto()
    {
        return ClientListItemDtoFaker.Generate();
    }
}