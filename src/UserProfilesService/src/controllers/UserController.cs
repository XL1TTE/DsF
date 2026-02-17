using Events.User;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Contorllers;

[ApiController, Route("user/")]
public class UserController(IMessageBus bus): Controller
{
    
    [HttpPost("register")]
    public ActionResult Register([FromBody] RegisterUser request)
    {
        bus.PublishAsync(new AccountRegistered
        {
            Time = DateTime.Now,
            ClientId = "Api",
            OperationType = "REGISTER"
        });
        
        return Ok();
    }
}
