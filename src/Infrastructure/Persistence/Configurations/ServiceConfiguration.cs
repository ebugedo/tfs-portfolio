// <copyright file="ServiceConfiguration.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tfs.Portfolio.Domain.Services.Entities;

/// <summary>
/// Entity configuration for Service.
/// </summary>
public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Services");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(s => s.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(150)")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.Description)
            .HasColumnName("Description")
            .HasColumnType("text")
            .HasMaxLength(1000);

        builder.Property(s => s.Category)
            .HasColumnName("Category")
            .HasColumnType("integer")
            .IsRequired();

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

        builder.HasIndex(s => s.Category)
            .HasDatabaseName("IX_Services_Category");

        builder.HasQueryFilter(s => s.IsActive);
    }
}