
using Commands;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Controllers;

[ApiController, Route("users/profiles")]
public class ProfilesController(IMessageBus bus): ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType<UserProfile>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById([FromRoute] string id)
    {
        return await bus.InvokeAsync<ActionResult>(new GetProfileById(id));
    }
    
    [HttpPost("{id}"), Authorize]
    [ProducesResponseType<UserProfile>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> UpdateById([FromBody] UpdateProfile request)
    {
        return await bus.InvokeAsync<ActionResult>(request);
    }
}
