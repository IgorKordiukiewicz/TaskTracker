using Analytics.Infrastructure.Models;

namespace Analytics.Infrastructure;

public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
    : DbContext(options)
{
    public DbSet<Event> Events { get; set; }

    public DbSet<DailyTotalTaskStatus> DailyTotalTaskStatuses { get; set; }
    public DbSet<DailyTotalTaskPriority> DailyTotalTaskPriorities { get; set; }
    public DbSet<DailyTotalTaskAssignee> DailyTotalTaskAssignees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("analytics");

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(e => e.ProjectId);
        });

        var projectionEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(IProjection).IsAssignableFrom(e.ClrType));
        foreach (var entityType in projectionEntityTypes)
        {
            modelBuilder.Entity(entityType.ClrType, entity =>
            {
                entity.HasKey(nameof(IProjection.Id));

                entity.HasIndex(nameof(IProjection.ProjectId));
            });
        }
    }
}
