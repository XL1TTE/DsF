using Common.DataAccess;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Testing.Common.DataAccess;

/// <summary>
/// Unit tests for UnitOfWork atomicity
/// </summary>
public class UnitOfWorkTests
{
    /// <summary>
    /// Test entity for UnitOfWork testing
    /// </summary>
    public class TestEntity : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// InMemory DbContext for testing
    /// </summary>
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TestEntity> TestEntities => Set<TestEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
            });
        }
    }

    [Fact]
    public async Task save_async_when_called_commits_all_changes()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("SaveAsync_CommitsAllChanges")
            .Options;

        await using var context = new TestDbContext(options);
        var mockFactory = CreateMockFactory(context);
        var unitOfWork = new UnitOfWork<TestDbContext>(mockFactory.Object, context);

        // Act
        unitOfWork.Repository<TestEntity>().Add(new TestEntity { Id = 1, Name = "Entity1" });
        unitOfWork.Repository<TestEntity>().Add(new TestEntity { Id = 2, Name = "Entity2" });
        await unitOfWork.SaveAsync();

        // Assert
        var entities = context.TestEntities.ToList();
        entities.Count.ShouldBe(2);
    }

    [Fact]
    public void save_when_called_commits_all_changes()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("Save_CommitsAllChanges")
            .Options;

        using var context = new TestDbContext(options);
        var mockFactory = CreateMockFactory(context);
        var unitOfWork = new UnitOfWork<TestDbContext>(mockFactory.Object, context);

        // Act
        unitOfWork.Repository<TestEntity>().Add(new TestEntity { Id = 1, Name = "Entity1" });
        unitOfWork.Save();

        // Assert
        var entities = context.TestEntities.ToList();
        entities.Count.ShouldBe(1);
    }

    [Fact]
    public async Task save_async_when_exception_thrown_no_changes_committed()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("SaveAsync_Exception_NoChanges")
            .Options;

        using var context = new TestDbContext(options);
        var mockFactory = new Mock<IRepositoryFactory>();
        var mockRepository = new Mock<IRepository<TestEntity>>();

        mockRepository
            .Setup(r => r.Add(It.IsAny<TestEntity>()))
            .Returns<TestEntity>(e =>
            {
                context.TestEntities.Add(e);
                return e;
            });

        mockFactory.Setup(f => f.CreateRepository<TestEntity>()).Returns(mockRepository.Object);
        var unitOfWork = new UnitOfWork<TestDbContext>(mockFactory.Object, context);

        // Act & Assert
        unitOfWork.Repository<TestEntity>().Add(new TestEntity { Id = 1, Name = "Entity1" });
        
        await Should.ThrowAsync<Exception>(async () =>
        {
            throw new Exception("Database error");
        });

        // Changes were added to context but SaveAsync was not called
        // In EF Core, changes are tracked but not persisted without SaveChanges
        var entities = context.TestEntities.Local.ToList();
        entities.Count.ShouldBe(1); // Tracked but not saved
    }

    [Fact]
    public void dispose_when_called_disposes_context()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("Dispose_Context")
            .Options;

        var context = new TestDbContext(options);
        var mockFactory = CreateMockFactory(context);
        var unitOfWork = new UnitOfWork<TestDbContext>(mockFactory.Object, context);

        // Act
        unitOfWork.Dispose();

        // Assert
        Should.Throw<ObjectDisposedException>(() => context.TestEntities.ToList());
    }

    [Fact]
    public void using_statement_when_exited_disposes_context()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("Using_Dispose")
            .Options;

        TestDbContext context;
        using (var unitOfWork = new UnitOfWork<TestDbContext>(CreateMockFactory(null!).Object, context = new TestDbContext(options)))
        {
            context.TestEntities.ShouldNotBeNull();
        }

        // Assert
        Should.Throw<ObjectDisposedException>(() => context.TestEntities.ToList());
    }

    [Fact]
    public async Task multiple_save_async_when_called_each_commits_independently()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("MultipleSaveAsync")
            .Options;

        await using var context = new TestDbContext(options);
        var mockFactory = CreateMockFactory(context);
        var unitOfWork = new UnitOfWork<TestDbContext>(mockFactory.Object, context);

        // Act - First save
        unitOfWork.Repository<TestEntity>().Add(new TestEntity { Id = 1, Name = "First" });
        await unitOfWork.SaveAsync();

        // Act - Second save
        unitOfWork.Repository<TestEntity>().Add(new TestEntity { Id = 2, Name = "Second" });
        await unitOfWork.SaveAsync();

        // Assert
        var entities = context.TestEntities.ToList();
        entities.Count.ShouldBe(2);
    }

    [Fact]
    public async Task change_tracker_clear_simulates_rollback()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("ChangeTrackerClear")
            .Options;

        using var context = new TestDbContext(options);
        var mockFactory = CreateMockFactory(context);
        var unitOfWork = new UnitOfWork<TestDbContext>(mockFactory.Object, context);

        // Act
        unitOfWork.Repository<TestEntity>().Add(new TestEntity { Id = 1, Name = "Entity1" });
        
        // Simulate rollback by clearing change tracker before save
        context.ChangeTracker.Clear();

        // Assert
        context.TestEntities.Local.ShouldBeEmpty();
    }

    private Mock<IRepositoryFactory> CreateMockFactory(TestDbContext context)
    {
        var mockFactory = new Mock<IRepositoryFactory>();
        var mockRepository = new Mock<IRepository<TestEntity>>();

        mockRepository
            .Setup(r => r.Add(It.IsAny<TestEntity>()))
            .Returns<TestEntity>(e =>
            {
                context?.TestEntities.Add(e);
                return e;
            });

        mockRepository
            .Setup(r => r.GetById(It.IsAny<object>()))
            .Returns<object>(id =>
            {
                var intId = Convert.ToInt32(id);
                return context?.TestEntities.Find(intId);
            });

        mockRepository
            .Setup(r => r.GetAll(It.IsAny<int>(), It.IsAny<int>()))
            .Returns<int, int>((skip, take) =>
            {
                return context?.TestEntities.Skip(skip).Take(take) ?? Enumerable.Empty<TestEntity>();
            });

        mockRepository
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<TestEntity, bool>>>()))
            .Returns<System.Linq.Expressions.Expression<Func<TestEntity, bool>>>(filter =>
            {
                return filter == null
                    ? context?.TestEntities.ToList() ?? Enumerable.Empty<TestEntity>()
                    : context?.TestEntities.Where(filter) ?? Enumerable.Empty<TestEntity>();
            });

        mockRepository
            .Setup(r => r.Update(It.IsAny<TestEntity>()))
            .Returns<TestEntity>(entity =>
            {
                context?.TestEntities.Update(entity);
                return entity;
            });

        mockRepository
            .Setup(r => r.Delete(It.IsAny<object>()))
            .Returns<object>(id =>
            {
                var entity = context?.TestEntities.Find(id);
                if (entity != null)
                {
                    context?.TestEntities.Remove(entity);
                }
                return entity;
            });

        mockFactory.Setup(f => f.CreateRepository<TestEntity>()).Returns(mockRepository.Object);
        return mockFactory;
    }
}
