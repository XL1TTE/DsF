namespace Common.DataAccess;

public interface IRepositoryFactory
{
    IRepository<T> CreateRepository<T>() where T: class, IDbEntity;
}
