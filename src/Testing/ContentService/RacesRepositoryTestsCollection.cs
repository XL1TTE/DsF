using Testing.ContentService.Fixtures;
using Xunit;

namespace Testing.ContentService;

[CollectionDefinition("RacesRepositoryTests")]
public class RacesRepositoryTestsCollection : ICollectionFixture<RaceDocumentFixture>
{
    
}
