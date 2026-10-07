// <copyright file="IClientRepository.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Clients.Repositories;

using Tfs.Portfolio.Domain.Clients.Entities;

/// <summary>
/// Repository interface for clients.
/// </summary>
public interface IClientRepository
{
    /// <summary>
    /// Gets a client by its identifier.
    /// </summary>
    /// <param name="id">The client identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The client if found; otherwise, null.</returns>
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all clients.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of clients.</returns>
    Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active clients.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of active clients.</returns>
    Task<IReadOnlyList<Client>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new client.
    /// </summary>
    /// <param name="client">The client to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Client client, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing client.
    /// </summary>
    /// <param name="client">The client to update.</param>
    void Update(Client client);

    /// <summary>
    /// Deletes a client.
    /// </summary>
    /// <param name="client">The client to delete.</param>
    void Delete(Client client);
}