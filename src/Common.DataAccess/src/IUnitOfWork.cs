namespace Common.DataAccess;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : class, IDbEntity;
    void Save();
    Task SaveAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}

