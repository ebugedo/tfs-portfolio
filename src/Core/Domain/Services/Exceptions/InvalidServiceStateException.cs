// <copyright file="InvalidServiceStateException.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Services.Exceptions;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Exception thrown when a service is in an invalid state.
/// </summary>
public sealed class InvalidServiceStateException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidServiceStateException"/> class.
    /// </summary>
    public InvalidServiceStateException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidServiceStateException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidServiceStateException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidServiceStateException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidServiceStateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}