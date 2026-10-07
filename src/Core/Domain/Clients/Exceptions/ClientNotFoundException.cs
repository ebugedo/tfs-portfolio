// <copyright file="ClientNotFoundException.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Clients.Exceptions;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Exception thrown when a client is not found.
/// </summary>
public sealed class ClientNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientNotFoundException"/> class.
    /// </summary>
    public ClientNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ClientNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientNotFoundException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ClientNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientNotFoundException"/> class with the client identifier.
    /// </summary>
    /// <param name="clientId">The client identifier that was not found.</param>
    public ClientNotFoundException(Guid clientId)
        : base($"Client with ID '{clientId}' was not found")
    {
        this.ClientId = clientId;
    }

    /// <summary>
    /// Gets the client identifier that was not found.
    /// </summary>
    public Guid ClientId { get; }
}