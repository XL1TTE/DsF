using Commands;
using Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using Persistence.Documents;
using Wolverine;

namespace Controllers;

[ApiController, Route("races/")]
public class RacesController(IMessageBus bus): ControllerBase
{
    [HttpGet("{name}")]
    [ProducesResponseType<RaceDocument>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetByName(string name, [FromQuery] string format = "application/json")
    {
        Response.Headers.ContentType = format;
        return await bus.InvokeAsync<ActionResult>(new GetRaceBySlug(name));
    }

    [HttpGet("all")]
    [ProducesResponseType<ICollection<RaceDocument>>(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        return await bus.InvokeAsync<ActionResult>(new GetAllRaces(skip, take));
    }

    [HttpPost("add")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<RaceDocument>(StatusCodes.Status201Created)]
    public async Task<ActionResult> AddNew([FromForm] CreateRaceRequest request)
    {
        return await bus.InvokeAsync<ActionResult>(request);
    }
}
