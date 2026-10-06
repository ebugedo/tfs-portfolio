// <copyright file="PersistenceModule.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Modules;

using Autofac;
using Microsoft.Extensions.Configuration;
using Tfs.Portfolio.Domain.Common;
using Tfs.Portfolio.Domain.Projects.Repositories;
using Tfs.Portfolio.Infrastructure.Persistence.Repositories;

/// <summary>
/// Autofac module for the Persistence layer.
/// </summary>
public sealed class PersistenceModule : Module
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceModule"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public PersistenceModule(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        // Register repositories
        builder.RegisterType<ProjectRepository>()
            .As<IProjectRepository>()
            .InstancePerLifetimeScope();

        // Register UnitOfWork
        builder.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();
    }
}