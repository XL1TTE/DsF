namespace Common.DataAccess;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T: class, IDbEntity;
    void Save();
    Task SaveAsync();
}

