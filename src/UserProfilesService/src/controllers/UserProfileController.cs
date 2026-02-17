using Handlers;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Contorllers;


[ApiController]
[Route("user/profiles")]
public class UserProfilesController(IMessageBus bus): Controller
{    
    [HttpPost("create")]
    public async Task<ActionResult<UserProfileCreated>> Create([FromBody] CreateUserProfile request)
    {
        var response = await bus.InvokeAsync<UserProfileCreated>(request);
        return Ok(response);
    }
}
