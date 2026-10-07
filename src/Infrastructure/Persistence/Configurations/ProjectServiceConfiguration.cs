// <copyright file="ProjectServiceConfiguration.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tfs.Portfolio.Domain.Projects.Entities;
using Tfs.Portfolio.Domain.Services.Entities;
using Tfs.Portfolio.Infrastructure.Persistence.Entities;

/// <summary>
/// Entity configuration for Project-Service many-to-many relationship.
/// </summary>
public sealed class ProjectServiceConfiguration : IEntityTypeConfiguration<ProjectService>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ProjectService> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("ProjectServices");

        builder.HasKey(ps => new { ps.ProjectId, ps.ServiceId });

        builder.Property(ps => ps.ProjectId)
            .HasColumnName("ProjectId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(ps => ps.ServiceId)
            .HasColumnName("ServiceId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(ps => ps.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ProjectServices_Projects");

        builder.HasOne<Service>()
            .WithMany()
            .HasForeignKey(ps => ps.ServiceId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ProjectServices_Services");

        builder.HasIndex(ps => ps.ProjectId)
            .HasDatabaseName("IX_ProjectServices_ProjectId");

        builder.HasIndex(ps => ps.ServiceId)
            .HasDatabaseName("IX_ProjectServices_ServiceId");
    }
}