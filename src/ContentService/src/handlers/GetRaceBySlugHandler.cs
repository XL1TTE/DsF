using Commands;
using Common.DataAccess;
using Contracts.Responses;
using ImTools;
using Microsoft.AspNetCore.Mvc;
using Persistence.Documents;
using Wolverine.Attributes;

namespace Handlers;

[WolverineHandler]
public class GetRaceBySlugHandler
{
    public async Task<ActionResult> Consume(GetRaceBySlug command, IUnitOfWork unit)
    {
        var race = unit.Repository<RaceDocument>().Get(x => x.Slug == command.slug).ToArray();

        if (race.IsNullOrEmpty()) { return new NotFoundResult(); }

        var result = race.Select(x => new RaceDto(x.Name, x.History, x.Health, x.PreviewUrl)).First();
        return new OkObjectResult(result);
    }
}
