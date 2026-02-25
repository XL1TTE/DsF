using Microsoft.Extensions.DependencyInjection;

namespace Common.DataAccess;

public class RepositoryFactory(IServiceProvider serviceProvider) : IRepositoryFactory
{
    IRepository<T> IRepositoryFactory.CreateRepository<T>()
    {
        return serviceProvider.GetRequiredService<IRepository<T>>();
    }
}
