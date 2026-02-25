using Commands;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Wolverine.Attributes;

namespace Handlers;

[WolverineHandler]
public class GetProfileByIdHandler
{
    public async Task<ActionResult> Consume(GetProfileById command, MongoDbContext db)
    {
        var profile = await db.Profiles.FirstOrDefaultAsync(x => x.UserId == command.id);
        if(profile == null)
        {
            return new NotFoundResult();
        }
        return new OkObjectResult(profile);
    }
}
