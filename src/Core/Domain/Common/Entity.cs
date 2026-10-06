// <copyright file="Entity.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Common;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Base class for all entities with a typed identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class Entity<TId>
    where TId : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    protected Entity(TId id)
    {
        this.Id = id;
    }

    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    [Key]
    public TId Id { get; protected set; } = default!;

    /// <inheritdoc />
    public override string ToString() => $"{this.GetType().Name} [Id={this.Id}]";
}
