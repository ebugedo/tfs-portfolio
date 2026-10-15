// <copyright file="CompanyProfileTestCollection.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests;

using Xunit;

/// <summary>
/// Collection definition for CompanyProfile tests to disable parallel execution.
/// </summary>
[CollectionDefinition("CompanyProfileTests", DisableParallelization = true)]
public sealed class CompanyProfileTestFixture : ICollectionFixture<CustomWebApplicationFactory>
{
}