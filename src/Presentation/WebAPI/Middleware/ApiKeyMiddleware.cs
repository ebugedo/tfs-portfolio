// <copyright file="ApiKeyMiddleware.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

/// <summary>
/// Middleware that validates the X-API-Key header on non-GET requests.
/// GET endpoints remain publicly accessible without authentication.
/// </summary>
internal sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ApiKeyMiddleware> logger;
    private readonly string? expectedApiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="configuration">The application configuration.</param>
    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger, IConfiguration configuration)
    {
        this.next = next;
        this.logger = logger;
        this.expectedApiKey = configuration["ApiKeySettings:MasterKey"];
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsWriteOperation(context.Request.Method))
        {
            await this.next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-API-Key", out StringValues providedKey) ||
            string.IsNullOrEmpty(this.expectedApiKey) ||
            !string.Equals(providedKey, this.expectedApiKey, StringComparison.Ordinal))
        {
            this.logger.LogDebug("API Key validation failed");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
            return;
        }

        await this.next(context);
    }

    private static bool IsWriteOperation(string method)
    {
        return !string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Extension methods for registering the API key middleware.
/// </summary>
internal static class ApiKeyMiddlewareExtensions
{
    /// <summary>
    /// Adds the API key middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseApiKey(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiKeyMiddleware>();
    }
}
