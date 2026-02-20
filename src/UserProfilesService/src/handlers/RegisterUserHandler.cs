using Configs;
using Loggers;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;

namespace Handlers;

[WolverineHandler]
public class RegisterUserHandlerTest
{
    public ActionResult Consume(RegisterUser request, ILogger<UserAccountEvents> logger)
    {
        logger.LogInformation($"RegisterUser handler: user {request} - wasn't registered.");
        return new OkObjectResult("User was not registered, because testing handler was called!");
    }
}

[WolverineHandler]
[WolverineIgnore]
public class RegisterUserHandler
{
    public ActionResult Consume(RegisterUser request, AuthUri authUri, ILogger<UserAccountEvents> logger)
    {
        logger.LogInformation($"[EVENT][USER] Redirected to auth server for registration.");
        logger.LogInformation($"\t[URL] {authUri.Get.ToString()}");
        return new RedirectResult(authUri.Get.ToString());
    }
}
