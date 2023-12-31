﻿using Domain.Organizations;
using Domain.Projects;
using System.Linq.Expressions;

namespace Application.Data.Repositories;

public class OrganizationRepository : IRepository<Organization>
{
    private readonly AppDbContext _dbContext;

    public OrganizationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Organization?> GetBy(Expression<Func<Organization, bool>> predicate)
        => await _dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .FirstOrDefaultAsync(predicate);

    public async Task<Organization?> GetById(Guid id)
        => await GetBy(x => x.Id == id);

    public async Task<bool> Exists(Expression<Func<Organization, bool>> predicate)
        => await _dbContext.Organizations
        .Include(x => x.Members)
        .Include(x => x.Invitations)
        .Include(x => x.Roles)
        .AnyAsync(predicate);

    public async Task Add(Organization entity)
    {
        _dbContext.Organizations.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Organization entity)
    {
        await _dbContext.AddRemoveChildEntities(entity.Members);
        await _dbContext.AddRemoveChildEntities(entity.Invitations);
        await _dbContext.AddRemoveChildEntities(entity.Roles);
        await _dbContext.SaveChangesAsync();
    }
}
