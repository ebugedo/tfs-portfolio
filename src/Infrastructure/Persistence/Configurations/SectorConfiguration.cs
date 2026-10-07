// <copyright file="SectorConfiguration.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tfs.Portfolio.Domain.Sectors.Entities;

/// <summary>
/// Entity configuration for Sector.
/// </summary>
public sealed class SectorConfiguration : IEntityTypeConfiguration<Sector>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Sector> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Sectors");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(s => s.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(100)")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasColumnName("Description")
            .HasColumnType("varchar(500)")
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("boolean")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("IX_Sectors_Name");

        builder.HasQueryFilter(s => s.IsActive);
    }
}