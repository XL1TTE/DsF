using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.DataAccess;

public static class ServiceCollectionExtentions
{
    public static void AddUnitOfWorkFor<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        services.TryAddScoped<IRepositoryFactory, RepositoryFactory>();
        services.TryAddScoped<IUnitOfWork, UnitOfWork<TContext>>();
    }
}

