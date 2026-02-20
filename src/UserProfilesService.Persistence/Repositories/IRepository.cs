namespace Persistence.Repositories;

/// <summary>
/// Base repository for working with MongoDB documents
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Add a new document
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get document by ID
    /// </summary>
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing document
    /// </summary>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete document by ID
    /// </summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all documents from collection
    /// </summary>
    IAsyncEnumerable<T> GetAllAsync(CancellationToken cancellationToken = default);
}
