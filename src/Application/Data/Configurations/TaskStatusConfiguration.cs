﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class TaskStatusConfiguration : BaseEntityTypeConfiguration<Domain.Workflows.TaskStatus>
{
    public override void Configure(EntityTypeBuilder<Domain.Workflows.TaskStatus> builder)
    {
        builder.HasKey(x => x.Id);

        base.Configure(builder);
    }
}