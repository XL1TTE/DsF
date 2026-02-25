using Common.DataAccess;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Documents;
using Persistence.Repositories;
using Shouldly;

namespace Testing.ContentService;

/// <summary>
/// Integration tests for RacesRepository with InMemory database
/// </summary>
public class RacesRepositoryTests : IAsyncLifetime
{
    private readonly ContentDbContext _context;
    private readonly RacesRepository _repository;

    public RacesRepositoryTests()
    {
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
        await _context.DisposeAsync();
    }

    [Fact]
    public void Add_WhenCalled_AddsEntityToDatabase()
    {
        // Arrange
        var race = new RaceDocument
        {
            Slug = "test-race",
            Name = "Test Race",
            History = "Test History"
        };

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
        var race = new RaceDocument
        {
            Slug = "test-race",
            Name = "Test Race",
            History = "Test History"
        };
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
        var race1 = new RaceDocument
        {
            Slug = "duplicate-slug",
            Name = "Race 1",
            History = "History 1"
        };
        _context.Races.Add(race1);
        _context.SaveChanges();

        var race2 = new RaceDocument
        {
            Slug = "duplicate-slug", // Same slug as race1
            Name = "Race 2",
            History = "History 2"
        };

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
        var race = new RaceDocument
        {
            Slug = "human",
            Name = "Human",
            History = "Versatile race"
        };
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
        var race = new RaceDocument
        {
            Slug = "orc",
            Name = "Orc",
            History = "Warrior race"
        };
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
        _repository.Add(new RaceDocument { Slug = "high-elf", Name = "High Elf", History = "Noble elves" });
        _repository.Add(new RaceDocument { Slug = "wood-elf", Name = "Wood Elf", History = "Forest elves" });
        _repository.Add(new RaceDocument { Slug = "dwarf", Name = "Dwarf", History = "Mountain dwellers" });
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
        _repository.Add(new RaceDocument { Slug = "race-one", Name = "Race One", History = "First" });
        _repository.Add(new RaceDocument { Slug = "race-two", Name = "Race Two", History = "Second" });
        _repository.Add(new RaceDocument { Slug = "race-three", Name = "Race Three", History = "Third" });
        _repository.Add(new RaceDocument { Slug = "race-four", Name = "Race Four", History = "Fourth" });
        _context.SaveChanges();

        // Act
        var result = _repository.GetAll(1, 2).ToList();

        // Assert
        result.Count.ShouldBe(2);
        result[0].Slug.ShouldBe("race-two");
        result[1].Slug.ShouldBe("race-three");
    }
}
