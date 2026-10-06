// <copyright file="CommandHandlerBase.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Application.Common.CQRS;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Base class for command handlers without a result.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <inheritdoc />
    public abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base class for command handlers with a result.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public abstract class CommandHandlerBase<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <inheritdoc />
    public abstract Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base class for query handlers.
/// </summary>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <inheritdoc />
    public abstract Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}