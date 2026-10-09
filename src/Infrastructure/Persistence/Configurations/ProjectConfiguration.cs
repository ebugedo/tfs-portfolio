// <copyright file="ProjectConfiguration.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tfs.Portfolio.Domain.Clients.Entities;
using Tfs.Portfolio.Domain.Common.ValueObjects;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Sectors.Entities;

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

        builder.Property(p => p.StartDate)
            .HasColumnName("StartDate")
            .HasColumnType("jsonb")
            .HasConversion(
                v => new { Month = v.Month, Year = v.Year },
                v => new YearMonth(v.Month, v.Year));

        builder.Property(p => p.DurationMonths)
            .HasColumnName("DurationMonths")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(p => p.ClientId)
            .HasColumnName("ClientId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(p => p.SectorId)
            .HasColumnName("SectorId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("Status")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(p => p.Technologies)
            .HasColumnName("Technologies")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v.Select(t => new { Name = t.Name, Category = t.Category.ToString(), Proficiency = t.Proficiency.ToString() }).ToList(),
                v => v.Select(t => Technology.Create(t.Name, Enum.Parse<TechnologyCategory>(t.Category), Enum.Parse<ProficiencyLevel>(t.Proficiency))).ToList())
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyList<Technology>>(
                (c1, c2) => (c1 ?? new List<Technology>()).SequenceEqual(c2 ?? new List<Technology>()),
                c => (c ?? new List<Technology>()).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));

        builder.Property(p => p.ServiceIds)
            .HasColumnName("ServiceIds")
            .HasColumnType("uuid[]")
            .HasConversion(
                v => v,
                v => v)
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyList<Guid>>(
                (c1, c2) => (c1 ?? new List<Guid>()).SequenceEqual(c2 ?? new List<Guid>()),
                c => (c ?? new List<Guid>()).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("timestamp with time zone");

        // Foreign keys
        builder.HasOne<Client>()
            .WithMany()
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Projects_Clients");

        builder.HasOne<Sector>()
            .WithMany()
            .HasForeignKey(p => p.SectorId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Projects_Sectors");

        // Indexes
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Projects_Name");

        builder.HasIndex(p => p.ClientId)
            .HasDatabaseName("IX_Projects_ClientId");

        builder.HasIndex(p => p.SectorId)
            .HasDatabaseName("IX_Projects_SectorId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Projects_Status");
    }
}