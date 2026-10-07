// <copyright file="IServiceRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Services.Repositories;

using Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// Repository interface for services.
/// </summary>
public interface IServiceRepository
{
    /// <summary>
    /// Gets a service by its identifier.
    /// </summary>
    /// <param name="id">The service identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The service if found; otherwise, null.</returns>
    Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all services.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of services.</returns>
    Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active services.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of active services.</returns>
    Task<IReadOnlyList<Service>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets services by category.
    /// </summary>
    /// <param name="category">The service category.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of services in the specified category.</returns>
    Task<IReadOnlyList<Service>> GetByCategoryAsync(ServiceCategory category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new service.
    /// </summary>
    /// <param name="service">The service to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Service service, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing service.
    /// </summary>
    /// <param name="service">The service to update.</param>
    void Update(Service service);

    /// <summary>
    /// Deletes a service.
    /// </summary>
    /// <param name="service">The service to delete.</param>
    void Delete(Service service);
}