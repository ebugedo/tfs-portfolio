// <copyright file="DateTimeProvider.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api;

using Tfs.Portfolio.Application.Common.Services;

/// <summary>
/// Implementation of date time provider using system clock.
/// </summary>
internal sealed class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc />
    public DateTime Now => DateTime.Now;

    /// <inheritdoc />
    public DateOnly TodayUtc => DateOnly.FromDateTime(DateTime.UtcNow);
}