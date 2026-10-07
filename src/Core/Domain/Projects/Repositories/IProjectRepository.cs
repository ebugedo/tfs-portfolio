// <copyright file="IProjectRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Projects.Repositories;

using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Repository interface for projects.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Gets a project by its identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The project if found; otherwise, null.</returns>
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all projects.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of projects.</returns>
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by client identifier.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of projects belonging to the client.</returns>
    Task<IReadOnlyList<Project>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by sector identifier.
    /// </summary>
    /// <param name="sectorId">The sector identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of projects belonging to the sector.</returns>
    Task<IReadOnlyList<Project>> GetBySectorIdAsync(Guid sectorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by technology name.
    /// </summary>
    /// <param name="technologyName">The technology name to search for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of projects containing the technology.</returns>
    Task<IReadOnlyList<Project>> GetByTechnologyAsync(string technologyName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by status.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of projects with the specified status.</returns>
    Task<IReadOnlyList<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active projects (status Active).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of active projects.</returns>
    Task<IReadOnlyList<Project>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new project.
    /// </summary>
    /// <param name="project">The project to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Project project, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="project">The project to update.</param>
    void Update(Project project);

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="project">The project to delete.</param>
    void Delete(Project project);
}