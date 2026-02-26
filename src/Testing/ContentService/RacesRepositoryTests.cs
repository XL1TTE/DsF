using Common.DataAccess;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Documents;
using Persistence.Repositories;
using Shouldly;
using Testing.ContentService.Fixtures;

namespace Testing.ContentService;

/// <summary>
/// Integration tests for RacesRepository with InMemory database
/// </summary>
[Collection("RacesRepositoryTests")]
public class RacesRepositoryTests : IAsyncLifetime
{
    private readonly ContentDbContext _context;
    private readonly RacesRepository _repository;
    private readonly RaceDocumentFixture _fixture;

    public RacesRepositoryTests(RaceDocumentFixture fixture)
    {
        _fixture = fixture;

        var options = new DbContextOptionsBuilder<ContentDbContext>()
            .UseInMemoryDatabase($"RacesTest_{Guid.NewGuid()}")
            .Options;

        _context = new ContentDbContext(options);
        _repository = new RacesRepository(_context);
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public void Add_WhenCalled_AddsEntityToDatabase()
    {
        // Arrange
        var race = _fixture.Create(slug: "test-race", name: "Test Race", history: "Test History");

        // Act
        var result = _repository.Add(race);
        _context.SaveChanges();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNullOrEmpty();

        var saved = _context.Races.Find(result.Id);
        saved.ShouldNotBeNull();
        saved.Name.ShouldBe("Test Race");
        saved.Slug.ShouldBe("test-race");
    }

    [Fact]
    public void update_when_called_updates_entity()
    {
        // Arrange
        var race = _fixture.Create(slug: "test-race", name: "Test Race", history: "Test History");
        _context.Races.Add(race);
        _context.SaveChanges();

        // Act - Get fresh entity from DB and update
        var existing = _context.Races.Find(race.Id);
        existing.ShouldNotBeNull();

        existing.Name = "Updated Race";
        existing.History = "Updated History";

        _repository.Update(existing);
        _context.SaveChanges();

        // Assert
        var saved = _context.Races.Find(race.Id!);
        saved.ShouldNotBeNull();
        saved.Name.ShouldBe("Updated Race");
        saved.History.ShouldBe("Updated History");
        saved.Slug.ShouldBe("test-race"); // Slug unchanged
    }

    [Fact]
    public void add_when_duplicate_slug_throws_exception()
    {
        // Arrange
        var race1 = _fixture.Create(slug: "duplicate-slug", name: "Race 1", history: "History 1");
        _context.Races.Add(race1);
        _context.SaveChanges();

        var race2 = _fixture.Create(slug: "duplicate-slug", name: "Race 2", history: "History 2");

        // Act & Assert - InMemory DB throws InvalidOperationException for duplicate alternate keys
        Should.Throw<InvalidOperationException>(() =>
        {
            _repository.Add(race2);
            _context.SaveChanges();
        });
    }

    [Fact]
    public void get_by_id_when_exists_returns_entity()
    {
        // Arrange
        var race = _fixture.Create(slug: "human", name: "Human", history: "Versatile race");
        _repository.Add(race);
        _context.SaveChanges();

        // Act
        var result = _repository.GetById(race.Id!);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Human");
        result.Slug.ShouldBe("human");
    }

    [Fact]
    public void get_by_id_when_not_exists_returns_null()
    {
        // Act
        var result = _repository.GetById("non-existent-id");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void delete_when_called_removes_entity()
    {
        // Arrange
        var race = _fixture.Create(slug: "orc", name: "Orc", history: "Warrior race");
        _repository.Add(race);
        _context.SaveChanges();

        // Act
        _repository.Delete(race.Id!);
        _context.SaveChanges();

        // Assert
        var saved = _context.Races.Find(race.Id!);
        saved.ShouldBeNull();
    }

    [Fact]
    public void get_with_filter_returns_filtered_entities()
    {
        // Arrange
        _repository.Add(_fixture.Create(slug: "high-elf", name: "High Elf", history: "Noble elves"));
        _repository.Add(_fixture.Create(slug: "wood-elf", name: "Wood Elf", history: "Forest elves"));
        _repository.Add(_fixture.Create(slug: "dwarf", name: "Dwarf", history: "Mountain dwellers"));
        _context.SaveChanges();

        // Act
        var result = _repository.Get(r => r.Slug.Contains("elf")).ToList();

        // Assert
        result.Count.ShouldBe(2);
        result.All(r => r.Slug.Contains("elf")).ShouldBeTrue();
    }

    [Fact]
    public void get_all_with_pagination_returns_correct_page()
    {
        // Arrange
        _repository.Add(_fixture.Create(slug: "race-one", name: "Race One", history: "First"));
        _repository.Add(_fixture.Create(slug: "race-two", name: "Race Two", history: "Second"));
        _repository.Add(_fixture.Create(slug: "race-three", name: "Race Three", history: "Third"));
        _repository.Add(_fixture.Create(slug: "race-four", name: "Race Four", history: "Fourth"));
        _context.SaveChanges();

        // Act
        var result = _repository.GetAll(1, 2).ToList();

        // Assert
        result.Count.ShouldBe(2);
        result[0].Slug.ShouldBe("race-two");
        result[1].Slug.ShouldBe("race-three");
    }
}
