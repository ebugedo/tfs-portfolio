// <copyright file="CompanyProfileConfiguration.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tfs.Portfolio.Domain.CompanyProfile.Entities;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Entity configuration for CompanyProfile (singleton).
/// </summary>
public sealed class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfileEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CompanyProfileEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("CompanyProfiles");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(c => c.CompanyName)
            .HasColumnName("CompanyName")
            .HasColumnType("varchar(200)")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.ContactEmail)
            .HasColumnName("ContactEmail")
            .HasColumnType("varchar(255)")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.ContactPhone)
            .HasColumnName("ContactPhone")
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(c => c.TfsWebUrl)
            .HasColumnName("TfsWebUrl")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v != null ? System.Text.Json.JsonSerializer.Serialize(v.Value.Value) : null,
                v => string.IsNullOrEmpty(v) ? default : TfsWebUrl.FromString(System.Text.Json.JsonSerializer.Deserialize<string>(v)));

        builder.Property(c => c.Address)
            .HasColumnName("Address")
            .HasColumnType("text");

        builder.Property(c => c.Description)
            .HasColumnName("Description")
            .HasColumnType("text");

        builder.Property(c => c.LogoUrl)
            .HasColumnName("LogoUrl")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v != null ? System.Text.Json.JsonSerializer.Serialize(v.Value.Value) : null,
                v => string.IsNullOrEmpty(v) ? default : TfsWebUrl.FromString(System.Text.Json.JsonSerializer.Deserialize<string>(v)));

        builder.Property(c => c.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("timestamp with time zone");

        // Singleton constraint: unique index on a constant column
        builder.HasIndex(c => c.Id)
            .IsUnique()
            .HasDatabaseName("IX_CompanyProfiles_Singleton");
    }
}