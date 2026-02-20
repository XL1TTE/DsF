using Documents;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

/// <summary>
/// Repository for working with user profiles
/// </summary>
public class ProfileRepository : IRepository<ProfileDocument>
{
    private readonly MongoDbContext _context;

    public ProfileRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ProfileDocument> AddAsync(ProfileDocument entity, CancellationToken cancellationToken = default)
    {
        await _context.Documents.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ProfileDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Documents.FindAsync([id], cancellationToken: cancellationToken);
    }

    public async Task<ProfileDocument> UpdateAsync(ProfileDocument entity, CancellationToken cancellationToken = default)
    {
        _context.Documents.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await GetByIdAsync(id, cancellationToken);
        if (document is not null)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async IAsyncEnumerable<ProfileDocument> GetAllAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var document in _context.Documents.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return document;
        }
    }
}
