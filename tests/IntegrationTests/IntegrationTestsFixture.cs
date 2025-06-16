using Application.Common;
using Infrastructure;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Linq.Expressions;
using Analytics.Infrastructure;
using Microsoft.AspNetCore.TestHost;

namespace IntegrationTests;

public class TestDateTimeProvider : IDateTimeProvider
{
    public DateTime Now() => new(2023, 10, 15);
}

public class IntegrationTestsFixture : IDisposable
{
    private readonly IServiceProvider _services;
    private readonly string _connectionString = "Host=localhost;Port=5432;Database=TaskTrackerDbTests;Username=postgres;Password=qwerty123"; // TODO: Store it somewhere else

    public IBackgroundJobClient BackgroundJobClientMock { get; } = Substitute.For<IBackgroundJobClient>();

    public IntegrationTestsFixture()
    {
        var appFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AppDbContext>))
                    ?? throw new Exception("DbContext service not found.");
                var analyticsContextDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<AnalyticsDbContext>))
                    ?? throw new Exception("AnalyticsDbContext service not found.");

                services.Remove(dbContextDescriptor);
                services.Remove(analyticsContextDescriptor);
                services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_connectionString));
                services.AddDbContext<AnalyticsDbContext>(options => options.UseNpgsql(_connectionString));

                services.AddScoped<IDateTimeProvider, TestDateTimeProvider>();
                services.AddScoped(serviceProvider => BackgroundJobClientMock);
            });

            builder.UseEnvironment("Development");
        });

        _services = appFactory.Services;
    }

    public async Task ExecuteOnService<T>(Func<T, Task> action) 
        where T : notnull
    {
        using var scope = _services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        await action(service);
    }

    public void ResetDb()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var db2 = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();
        db2.Database.Migrate();
    }

    public async Task SeedDb(Action<AppDbContext> action)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        action(db);
        await db.SaveChangesAsync();
    }

    public void Dispose()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
    }

    public async Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(request);
    }

    public async Task SendRequest(IRequest request)
    {
        using var scope = _services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(request);
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