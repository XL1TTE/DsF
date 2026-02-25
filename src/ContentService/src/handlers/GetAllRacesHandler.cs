using Commands;
using Common.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Persistence.Documents;
using Wolverine.Attributes;

namespace Handlers;

[WolverineHandler]
public class GetAllRacesHandler
{
    public async Task<ActionResult> Consume(GetAllRaces command, IUnitOfWork unit)
    {
        var races = unit.Repository<RaceDocument>().GetAll(command.Skip, command.Take);
        
        var result = from race in races
                     select new {race.Id, race.Name, race.History};

        return new OkObjectResult(result);
    }
}
