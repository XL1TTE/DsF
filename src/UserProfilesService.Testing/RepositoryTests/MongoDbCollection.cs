using Testing.Fixtures;

namespace Testing.RepositoryTests;

/// <summary>
/// Test collection for repository tests with shared MongoDB environment
/// </summary>
[CollectionDefinition("MongoDB Collection")]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
    // This class contains no code, only collection definition
}
