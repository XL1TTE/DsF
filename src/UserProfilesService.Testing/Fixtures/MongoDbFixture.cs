using System.Web;
using Testcontainers.MongoDb;

namespace Testing.Fixtures;

/// <summary>
/// Fixture for managing MongoDB container in tests
/// </summary>
public sealed class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container;

    public MongoDbFixture()
    {
        _container = new MongoDbBuilder("mongo:8").Build();
    }

    /// <summary>
    /// Connection string for connecting to test database
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        ConnectionString = _container.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
