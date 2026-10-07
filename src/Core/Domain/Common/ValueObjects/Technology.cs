// <copyright file="Technology.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Represents a technology category.
/// </summary>
public enum TechnologyCategory
{
    /// <summary>Frontend technologies (React, Angular, Vue, etc.)</summary>
    Frontend,

    /// <summary>Backend technologies (C#, Java, Node.js, Python, etc.)</summary>
    Backend,

    /// <summary>Database technologies (PostgreSQL, MongoDB, Redis, etc.)</summary>
    Database,

    /// <summary>Cloud technologies (AWS, Azure, GCP, Kubernetes, etc.)</summary>
    Cloud,

    /// <summary>Tools and utilities (Docker, Git, CI/CD, etc.)</summary>
    Tools,
}

/// <summary>
/// Represents a proficiency level.
/// </summary>
public enum ProficiencyLevel
{
    /// <summary>Beginner level</summary>
    Beginner,

    /// <summary>Intermediate level</summary>
    Intermediate,

    /// <summary>Expert level</summary>
    Expert,
}

/// <summary>
/// Represents a technology with name, category, and proficiency.
/// Immutable value object with case-insensitive equality by name.
/// </summary>
public readonly struct Technology : IEquatable<Technology>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Technology"/> struct.
    /// </summary>
    /// <param name="name">The technology name.</param>
    /// <param name="category">The technology category.</param>
    /// <param name="proficiency">The proficiency level.</param>
    /// <exception cref="ArgumentException">Thrown when name is null or whitespace.</exception>
    public Technology(string name, TechnologyCategory category, ProficiencyLevel proficiency)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Technology name cannot be null or whitespace.", nameof(name));
        }

        this.Name = name.Trim();
        this.Category = category;
        this.Proficiency = proficiency;
    }

    /// <summary>
    /// Gets the technology name (e.g., "React", "PostgreSQL").
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the technology category.
    /// </summary>
    public TechnologyCategory Category { get; }

    /// <summary>
    /// Gets the proficiency level.
    /// </summary>
    public ProficiencyLevel Proficiency { get; }

    /// <summary>
    /// Compares two <see cref="Technology"/> instances for equality (case-insensitive by name).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if names are equal ignoring case.</returns>
    public static bool operator ==(Technology left, Technology right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="Technology"/> instances for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True if names are not equal ignoring case.</returns>
    public static bool operator !=(Technology left, Technology right) => !left.Equals(right);

    /// <summary>
    /// Creates a new <see cref="Technology"/> instance.
    /// </summary>
    /// <param name="name">The technology name.</param>
    /// <param name="category">The technology category.</param>
    /// <param name="proficiency">The proficiency level.</param>
    /// <returns>A new <see cref="Technology"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or whitespace.</exception>
    public static Technology Create(string name, TechnologyCategory category, ProficiencyLevel proficiency)
        => new(name, category, proficiency);

    /// <summary>
    /// Determines whether the current instance is equal to another instance.
    /// Equality is based on case-insensitive name comparison.
    /// </summary>
    /// <param name="other">The other technology to compare.</param>
    /// <returns>True if names are equal ignoring case.</returns>
    public bool Equals(Technology other)
        => string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether the current instance is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if obj is a Technology with equal name (case-insensitive).</returns>
    public override bool Equals(object? obj)
        => obj is Technology other && this.Equals(other);

    /// <summary>
    /// Gets the hash code based on the name (case-insensitive).
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
        => this.Name.GetHashCode(StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns a string representation.
    /// </summary>
    /// <returns>The technology name with category and proficiency.</returns>
    public override string ToString() => $"{this.Name} ({this.Category}, {this.Proficiency})";
}