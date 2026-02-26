using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.DataAccess;

public class UnitOfWork<TContext>(IRepositoryFactory factory, TContext context) : IUnitOfWork
where TContext : DbContext
{
    private IDbContextTransaction? _transaction;

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

    public void BeginTransaction()
    {
        _transaction = context.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        if (_transaction != null)
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void RollbackTransaction()
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }
    }
}

