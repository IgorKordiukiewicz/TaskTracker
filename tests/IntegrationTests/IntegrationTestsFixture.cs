using Application.Data;
using Domain.Common;
using FluentAssertions.Common;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace IntegrationTests;

public class IntegrationTestsFixture : IDisposable
{
    private readonly HttpClient _httpClient = new();
    private readonly IServiceProvider _services;
    private readonly string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=TaskTrackerDbTests;Trusted_Connection=True"; // TODO: Store it somewhere else?

    public IntegrationTestsFixture()
    {
        var appFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>))
                    ?? throw new Exception("DbContext service not found.");

                services.Remove(dbContextDescriptor);
                services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_connectionString));
            });

            builder.UseEnvironment("Development");
        });

        _httpClient = appFactory.CreateClient();
        _services = appFactory.Services;
    }

    public void ResetDb()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    public async Task SeedDb(Action<AppDbContext> action)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        action(db);
        await db.SaveChangesAsync();
    }

    public async Task AddEntities<TEntity>(params TEntity[] entities)
        where TEntity : class
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Set<TEntity>().AddRange(entities);
        await db.SaveChangesAsync();
    }

    public async Task CommitDbSeeding()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.SaveChangesAsync();
    }

    public void Dispose()
    {
        ResetDb();
    }

    public async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(request);
    }

    public async Task<List<TEntity>> GetAsync<TEntity>() 
        where TEntity : class
    {
        return await GetAsync<TEntity>(x => true);
    }

    public async Task<List<TEntity>> GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) 
        where TEntity : class
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async Task<TEntity> FirstAsync<TEntity>()
        where TEntity : class
    {
        return await FirstAsync<TEntity>(x => true);
    }

    public async Task<TEntity> FirstAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Set<TEntity>().Where(predicate).FirstAsync();
    }

    public async Task<int> CountAsync<TEntity>()
        where TEntity : class
    {
        return await CountAsync<TEntity>(x => true);
    }

    public async Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Set<TEntity>().CountAsync(predicate);
    }
}

[CollectionDefinition(nameof(IntegrationTestsCollection))]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}