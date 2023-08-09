using Domain.Common;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationInvitation> OrganizationInvitations { get; set; }
    public DbSet<OrganizationMember> OrganizationMembers { get; set; }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public async Task AddRemoveDependents<TDependent>(IEnumerable<TDependent> actualEntities)
        where TDependent : Entity
    {
        var dbEntities = await Set<TDependent>().AsNoTracking().Select(x => x.Id).ToListAsync();

        var addedEntities = actualEntities.Where(x => !dbEntities.Any(y => y == x.Id));
        Set<TDependent>().AddRange(addedEntities);

        var removedEntitiesIds = dbEntities.Where(x => !actualEntities.Any(y => y.Id == x));
        Set<TDependent>().RemoveRange(Set<TDependent>().Where(x => removedEntitiesIds.Contains(x.Id)));
    }
}
