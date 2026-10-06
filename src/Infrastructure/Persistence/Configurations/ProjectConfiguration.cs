// <copyright file="ProjectConfiguration.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tfs.Portfolio.Domain.Projects.Entities;

/// <summary>
/// Entity configuration for Project.
/// </summary>
public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(200)")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasColumnName("Description")
            .HasColumnType("text")
            .HasMaxLength(2000);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Projects_Name");
    }
}