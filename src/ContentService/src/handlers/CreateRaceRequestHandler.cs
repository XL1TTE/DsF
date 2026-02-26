using Common.Utilities.FileUpload;
using ContentService.Commands;
using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Attributes;

namespace ContentService.Handlers;

[WolverineHandler]
public class CreateRaceRequestHandler
{
    public async Task<ActionResult> Consume(CreateRaceRequest req, IMessageBus bus)
    {
        try
        {
            var preview = await req.Preview.UploadAsImageAsync("uploads/races");

            var raceDto = await bus.InvokeAsync<RaceDto>(
                new CreateRaceRecord(
                    req.Name,
                    req.History,
                    req.Health,
                    preview
                )
            );

            return new OkObjectResult(raceDto);
        }
        catch(NotAnImageException e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }
}
