using Commands;
using Common.DataAccess;
using Common.Utilities;
using ContentService.Contracts;
using Exceptions;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using Persistence.Documents;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

namespace Handlers;

[WolverineHandler]
public class CreateNewRaceRequestHandler
{
    public static void Configure(HandlerChain chain)
    {
        chain.OnException<FailedToAddRaceException>()
            .Discard().And(async (_, context, e) =>
            {
                await context.RespondToSenderAsync(new StatusCodeResult(505));
            });
    }

    public async Task<ActionResult> Consume(CreateNewRaceRequest command, IUnitOfWork unit)
    {
        var repository = unit.Repository<RaceDocument>();
        var race = repository.Add(new RaceDocument
        {
            Name = command.name,
            History = command.history,
            Slug = command.name.Slugify()
        });
        
        try
        {
            await unit.SaveAsync();   
            return new OkObjectResult(race);
        }
        catch
        {
            throw new FailedToAddRaceException();
        }
    }
}
