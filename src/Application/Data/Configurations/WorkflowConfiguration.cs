﻿using Domain.Projects;
using Domain.Workflows;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class WorkflowConfiguration : BaseEntityTypeConfiguration<Workflow>
{
    public override void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<Project>()
            .WithOne()
            .HasForeignKey<Workflow>(x => x.ProjectId);

        builder.HasMany(x => x.Statuses)
            .WithOne();

        builder.HasMany(x => x.Transitions)
            .WithOne();

        base.Configure(builder);
    }
}
