using System.Linq.Expressions;

namespace Common.DataAccess;

public interface IRepository<T> where T: class, IDbEntity
{
    /// <summary>
    /// Gets all entities that meet filter criteria.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>Filtered entities.</returns>
    IEnumerable<T> Get(Expression<Func<T, bool>>? filter = null);
    
    /// <summary>
    /// Gets all entities with pagination.
    /// </summary>
    /// <param name="skip">Amount of entities to skip.</param>
    /// <param name="take">Amount of entities to return.</param>
    /// <returns>Entities from [skip, skip + pick]</returns>
    IEnumerable<T> GetAll(int skip, int take);
    
    /// <summary>
    /// Gets entity with specified id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Entity.</returns>
    T? GetById(object id);

    /// <summary>
    /// Inserts new record in database.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>Inserted entity.</returns>
    T Add(T entity);

    /// <summary>
    /// Deletes entity from database by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Deleted entity.</returns>
    T? Delete(object id);
    
    /// <summary>
    /// Updates entity in database.
    /// </summary>
    /// <returns>Updated entity.</returns>
    T? Update(T entity);
    
}
