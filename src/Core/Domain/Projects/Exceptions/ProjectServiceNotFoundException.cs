// <copyright file="ProjectServiceNotFoundException.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Exceptions;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Exception thrown when a service is not found in a project.
/// </summary>
public sealed class ProjectServiceNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectServiceNotFoundException"/> class.
    /// </summary>
    public ProjectServiceNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectServiceNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ProjectServiceNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectServiceNotFoundException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ProjectServiceNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectServiceNotFoundException"/> class with the project and service identifiers.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="serviceId">The service identifier that was not found in the project.</param>
    public ProjectServiceNotFoundException(Guid projectId, Guid serviceId)
        : base($"Service with ID '{serviceId}' was not found in project with ID '{projectId}'")
    {
        this.ProjectId = projectId;
        this.ServiceId = serviceId;
    }

    /// <summary>
    /// Gets the project identifier.
    /// </summary>
    public Guid ProjectId { get; }

    /// <summary>
    /// Gets the service identifier that was not found.
    /// </summary>
    public Guid ServiceId { get; }
}