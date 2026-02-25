using Microsoft.EntityFrameworkCore;

namespace Common.DataAccess;

public class UnitOfWork<TContext>(IRepositoryFactory factory, TContext context) : IUnitOfWork
where TContext : DbContext
{
    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IDbEntity
    {
        return factory.CreateRepository<TEntity>();
    }

    public void Save()
    {
        context.SaveChanges();
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }

    public void Dispose() => context.Dispose(); 
}

