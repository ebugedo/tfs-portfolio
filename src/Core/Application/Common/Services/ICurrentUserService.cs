// <copyright file="ICurrentUserService.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Common.Services;

/// <summary>
/// Service for accessing current user information.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user identifier.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user name.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if the current user has the specified role.
    /// </summary>
    /// <param name="role">The role to check.</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    bool IsInRole(string role);
}