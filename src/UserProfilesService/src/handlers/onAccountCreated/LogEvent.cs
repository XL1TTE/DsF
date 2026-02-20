using Events.User;
using Loggers;
using Wolverine.Attributes;

namespace Handlers;

[WolverineHandler]
public class LogEvent
{
    public async Task Consume(AccountRegistered @event, ILogger<ProfileEvents> logger)
    {
        logger.LogInformation($"[Event] New user account created at {@event.Time}.\n\tInfo: {@event}");
    }
}
