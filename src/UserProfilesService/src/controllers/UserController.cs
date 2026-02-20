using Configs;
using Events.User;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Contorllers;

[ApiController, Route("user/")]
public class UserController(IMessageBus bus) : Controller
{
    [HttpPost("register")]
    public async Task<ActionResult> Register()
    {
        var response = await bus.InvokeAsync<ActionResult>(new RegisterUser());

        return response;
    }
}
