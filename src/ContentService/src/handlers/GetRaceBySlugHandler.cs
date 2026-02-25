using Commands;
using Common.DataAccess;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using Persistence;
using Persistence.Documents;
using Wolverine.Attributes;

namespace Handlers;


[WolverineHandler]
public class GetRaceBySlugHandler
{
    public async Task<ActionResult> Consume(GetRaceBySlug command, IUnitOfWork unit)
    {
        var race = unit.Repository<RaceDocument>().Get(x => x.Slug == command.slug);
        
        if(race == null){return new NotFoundResult();}
        return new OkObjectResult(race.Select(x => new {x.Id, x.Name, x.History}));
    }
}
