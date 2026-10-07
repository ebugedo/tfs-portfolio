// <copyright file="ServiceCategory.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// Represents a service category.
/// </summary>
public enum ServiceCategory
{
    /// <summary>Development services</summary>
    Development,

    /// <summary>Consulting services</summary>
    Consulting,

    /// <summary>Design services</summary>
    Design,

    /// <summary>DevOps services</summary>
    DevOps,

    /// <summary>Training services</summary>
    Training,
}