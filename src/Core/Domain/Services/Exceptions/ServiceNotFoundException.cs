// <copyright file="ServiceNotFoundException.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Services.Exceptions;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Exception thrown when a service is not found.
/// </summary>
public sealed class ServiceNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
    /// </summary>
    public ServiceNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ServiceNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ServiceNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class with the service identifier.
    /// </summary>
    /// <param name="serviceId">The service identifier that was not found.</param>
    public ServiceNotFoundException(Guid serviceId)
        : base($"Service with ID '{serviceId}' was not found")
    {
        this.ServiceId = serviceId;
    }

    /// <summary>
    /// Gets the service identifier that was not found.
    /// </summary>
    public Guid ServiceId { get; }
}