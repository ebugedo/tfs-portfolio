// <copyright file="ProjectStatus.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Represents the status of a project.
/// </summary>
public enum ProjectStatus
{
    /// <summary>Project is in draft state.</summary>
    Draft,

    /// <summary>Project is active.</summary>
    Active,

    /// <summary>Project is on hold.</summary>
    OnHold,

    /// <summary>Project is completed.</summary>
    Completed,

    /// <summary>Project is cancelled.</summary>
    Cancelled,
}