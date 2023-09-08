using Domain.Projects;
using Domain.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class TaskStatesManagerConfiguration : IEntityTypeConfiguration<TaskStatesManager>
{
    public void Configure(EntityTypeBuilder<TaskStatesManager> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<Project>()
            .WithOne()
            .HasForeignKey<TaskStatesManager>(x => x.ProjectId);

        builder.HasMany(x => x.AllStates)
            .WithOne();
    }
}
