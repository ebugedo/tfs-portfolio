// <copyright file="ClientConfiguration.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Entity configuration for Client.
/// </summary>
public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Clients");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(c => c.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(200)")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(255)")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.LogoUrl)
            .HasColumnName("LogoUrl")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? Url.Create(v) : null);

        builder.Property(c => c.Phone)
            .HasColumnName("Phone")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(c => c.Address)
            .HasColumnName("Address")
            .HasColumnType("text");

        builder.Property(c => c.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("boolean")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Clients_Name");

        builder.HasIndex(c => c.Email)
            .HasDatabaseName("IX_Clients_Email");

        builder.HasQueryFilter(c => c.IsActive);
    }
}