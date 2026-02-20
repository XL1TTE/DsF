
using Documents;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Shouldly;
using Testing.Fixtures;

namespace Testing.RepositoryTests;

/// <summary>
/// Integration tests for ProfileRepository with MongoDB
/// </summary>
[Collection("MongoDB Collection")]
public class repository_should_handle_data_correctly
{
    private readonly MongoDbContext _context;
    private readonly ProfileRepository _repository;

    public repository_should_handle_data_correctly(MongoDbFixture fixture)
    {
        var options = new DbContextOptionsBuilder<MongoDbContext>()
            .UseMongoDB(fixture.ConnectionString, "testdb")
            .Options;

        _context = new MongoDbContext(options);
        _repository = new ProfileRepository(_context);
    }

    [Fact]
    public async Task add_call_should_create_new_record_if_id_unique()
    {
        // Arrange
        var profile = new ProfileDocument
        {
            Id = Guid.NewGuid().ToString(),
            Username = "testuser",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(profile);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(profile.Id);
        result.Username.ShouldBe(profile.Username);
        result.Email.ShouldBe(profile.Email);

        // Verify in database
        var fromDb = await _repository.GetByIdAsync(profile.Id);
        fromDb.ShouldNotBeNull();
        fromDb.Id.ShouldBe(profile.Id);
    }

    [Fact]
    public async Task get_by_id_should_return_null_if_not_found()
    {
        // Act
        var result = await _repository.GetByIdAsync("non-existent-id");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task update_call_should_modify_existing_record()
    {
        // Arrange
        var profile = new ProfileDocument
        {
            Id = Guid.NewGuid().ToString(),
            Username = "originaluser",
            Email = "original@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(profile);

        // Act
        profile.Username = "updateduser";
        profile.Email = "updated@example.com";
        var result = await _repository.UpdateAsync(profile);

        // Assert
        result.Username.ShouldBe("updateduser");
        result.Email.ShouldBe("updated@example.com");

        // Verify in database
        var fromDb = await _repository.GetByIdAsync(profile.Id);
        fromDb.ShouldNotBeNull();
        fromDb.Username.ShouldBe("updateduser");
        fromDb.Email.ShouldBe("updated@example.com");
    }

    [Fact]
    public async Task delete_call_should_remove_record()
    {
        // Arrange
        var profile = new ProfileDocument
        {
            Id = Guid.NewGuid().ToString(),
            Username = "tobedeleted",
            Email = "delete@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(profile);

        // Act
        await _repository.DeleteAsync(profile.Id);

        // Assert
        var fromDb = await _repository.GetByIdAsync(profile.Id);
        fromDb.ShouldBeNull();
    }

    [Fact]
    public async Task delete_call_should_not_fail_for_non_existent_id()
    {
        // Act & Assert (should not throw exception)
        await Should.NotThrowAsync(async () => await _repository.DeleteAsync("non-existent-id"));
    }

    [Fact]
    public async Task get_all_should_return_all_documents()
    {
        // Arrange
        var profile1 = new ProfileDocument
        {
            Id = Guid.NewGuid().ToString(),
            Username = "user1",
            Email = "user1@example.com",
            CreatedAt = DateTime.UtcNow
        };
        var profile2 = new ProfileDocument
        {
            Id = Guid.NewGuid().ToString(),
            Username = "user2",
            Email = "user2@example.com",
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(profile1);
        await _repository.AddAsync(profile2);

        // Act
        var results = new List<ProfileDocument>();
        await foreach (var profile in _repository.GetAllAsync())
        {
            results.Add(profile);
        }

        // Assert
        results.ShouldContain(p => p.Id == profile1.Id);
        results.ShouldContain(p => p.Id == profile2.Id);
    }
}
