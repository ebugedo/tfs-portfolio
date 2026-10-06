// <copyright file="ProjectFaker.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.UnitTests.Common;

using Bogus;
using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Faker for generating test Project entities.
/// </summary>
public sealed class ProjectFaker : Faker<Project>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFaker"/> class.
    /// </summary>
    public ProjectFaker()
    {
        this.CustomInstantiator(f => Project.Create(
            f.Commerce.ProductName(),
            f.Lorem.Paragraph()));
    }
}