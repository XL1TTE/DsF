using Testing.Fixtures;

namespace Testing;

/// <summary>
/// Collection fixture for MongoDB tests
/// Ensures MongoDB container is shared across tests in the same collection
/// </summary>
[CollectionDefinition("MongoDB Collection")]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
    // This class has no code; it is used to apply the collection definition
}
