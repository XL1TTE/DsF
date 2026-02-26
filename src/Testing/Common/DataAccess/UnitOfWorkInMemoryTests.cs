using Common.DataAccess;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Testing.Common.DataAccess;

/// <summary>
/// Unit tests for UnitOfWork with InMemory database
/// </summary>
public class UnitOfWorkInMemoryTests : IAsyncLifetime
{
    private TestDbContext _context = null!;
    private IRepositoryFactory _factory = null!;
    private UnitOfWork<TestDbContext> _unitOfWork = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new TestDbContext(options);
        _factory = new TestRepositoryFactory(_context);
        _unitOfWork = new UnitOfWork<TestDbContext>(_factory, _context);
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task save_async_when_called_commits_all_changes()
    {
        // Arrange
        var repository = _unitOfWork.Repository<TestEntity>();
        var entity1 = new TestEntity { Id = 1, Name = "Entity1" };
        var entity2 = new TestEntity { Id = 2, Name = "Entity2" };

        // Act
        repository.Add(entity1);
        repository.Add(entity2);
        await _unitOfWork.SaveAsync();

        // Assert
        var entities = _context.TestEntities.ToList();
        entities.Count.ShouldBe(2);
    }

    [Fact]
    public void save_when_called_commits_all_changes()
    {
        // Arrange
        var repository = _unitOfWork.Repository<TestEntity>();
        var entity = new TestEntity { Id = 1, Name = "Entity1" };

        // Act
        repository.Add(entity);
        _unitOfWork.Save();

        // Assert
        var entities = _context.TestEntities.ToList();
        entities.Count.ShouldBe(1);
    }

    [Fact]
    public async Task multiple_save_async_when_called_each_commits_independently()
    {
        // Arrange
        var repository = _unitOfWork.Repository<TestEntity>();

        // Act - First save
        repository.Add(new TestEntity { Id = 1, Name = "First" });
        await _unitOfWork.SaveAsync();

        // Act - Second save
        repository.Add(new TestEntity { Id = 2, Name = "Second" });
        await _unitOfWork.SaveAsync();

        // Assert
        var entities = _context.TestEntities.ToList();
        entities.Count.ShouldBe(2);
    }
}
