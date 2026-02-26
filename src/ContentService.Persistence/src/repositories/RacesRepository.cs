using System.Linq.Expressions;
using Common.DataAccess;
using Persistence.Contexts;
using Persistence.Documents;

namespace Persistence.Repositories;

public class RacesRepository(ContentDbContext context) : IRepository<RaceDocument>
{
    public RaceDocument Add(RaceDocument entity)
    {
        var entry = context.Races.Add(entity);
        return entry.Entity;
    }

    public RaceDocument? Delete(object id)
    {
        var entry = context.Races.Find(id);
        entry = entry != null ? context.Races.Remove(entry).Entity : null;
        return entry;
    }

    public IEnumerable<RaceDocument> Get(Expression<Func<RaceDocument, bool>>? filter = null)
    {
        var query = context.Races.AsQueryable();
        return filter != null ? query.Where(filter) : query;
    }

    public IEnumerable<RaceDocument> GetAll(int skip, int take)
    {
        return context.Races.Skip(skip).Take(take);
    }

    public RaceDocument? GetById(object id)
    {
        return context.Races.Find(id);
    }

    public RaceDocument? Update(RaceDocument entity)
    {
        var entry = context.Races.Update(entity);
        return entry.Entity;
    }
}
