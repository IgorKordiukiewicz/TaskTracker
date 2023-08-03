using Application.Data;
using Domain.Common;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        });

        _httpClient = appFactory.CreateClient();
        _services = appFactory.Services;

        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
        // TODO: Don't delete it, instead remove all data?
    }

    public IReadOnlyList<TEntity> GetAsync<TEntity>() where TEntity : class
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return db.Set<TEntity>().ToList();
    }
}

[CollectionDefinition(nameof(IntegrationTestsCollection))]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}