// <copyright file="ProjectNotFoundException.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Exceptions;

using Tfs.Portfolio.Domain.Common;

/// <summary>
/// Exception thrown when a project is not found.
/// </summary>
public sealed class ProjectNotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectNotFoundException"/> class.
    /// </summary>
    public ProjectNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ProjectNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectNotFoundException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ProjectNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectNotFoundException"/> class with the project identifier.
    /// </summary>
    /// <param name="projectId">The project identifier that was not found.</param>
    public ProjectNotFoundException(Guid projectId)
        : base($"Project with ID '{projectId}' was not found")
    {
        this.ProjectId = projectId;
    }

    /// <summary>
    /// Gets the project identifier that was not found.
    /// </summary>
    public Guid ProjectId { get; }
}