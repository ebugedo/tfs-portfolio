// <copyright file="CurrentUserService.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api;

using Microsoft.AspNetCore.Http;
using Tfs.Portfolio.Application.Common.Services;

/// <summary>
/// Implementation of current user service using HttpContext.
/// </summary>
internal sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public Guid? UserId =>
        this.httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value is string sub && Guid.TryParse(sub, out var id)
            ? id
            : null;

    /// <inheritdoc />
    public string? UserName =>
        this.httpContextAccessor.HttpContext?.User?.Identity?.Name;

    /// <inheritdoc />
    public bool IsAuthenticated =>
        this.httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public bool IsInRole(string role) =>
        this.httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
}