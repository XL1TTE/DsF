using Common.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Testing.Common.DataAccess;

/// <summary>
/// Test entity for UnitOfWork testing
/// </summary>
public class TestEntity : IDbEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Test DbContext for UnitOfWork testing
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

/// <summary>
/// Test repository factory
/// </summary>
public class TestRepositoryFactory : IRepositoryFactory
{
    private readonly TestDbContext _context;

    public TestRepositoryFactory(TestDbContext context)
    {
        _context = context;
    }

    public IRepository<T> CreateRepository<T>() where T : class, IDbEntity
    {
        return new TestRepository<T>(_context);
    }
}

/// <summary>
/// Generic repository for testing
/// </summary>
public class TestRepository<T> : IRepository<T> where T : class, IDbEntity
{
    private readonly TestDbContext _context;
    private readonly DbSet<T> _dbSet;

    public TestRepository(TestDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IEnumerable<T> Get(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null)
    {
        return filter == null ? _dbSet.ToList() : _dbSet.Where(filter).ToList();
    }

    public IEnumerable<T> GetAll(int skip, int take)
    {
        return _dbSet.Skip(skip).Take(take).ToList();
    }

    public T? GetById(object id)
    {
        return _dbSet.Find(id);
    }

    public T? Add(T entity)
    {
        _dbSet.Add(entity);
        return entity;
    }

    public T? Delete(object id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
        return entity;
    }

    public T? Update(T entity)
    {
        _dbSet.Update(entity);
        return entity;
    }
}
