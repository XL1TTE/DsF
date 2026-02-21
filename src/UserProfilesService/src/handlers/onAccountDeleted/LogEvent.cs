

using Events.User;
using Loggers;
using Wolverine.Attributes;

namespace Handlers.OnAccountDeleted;

[WolverineHandler]
public class LogEvent {

    public static async Task Consume(AccountDeleted @event, ILogger<ProfileEvents> logger)
    {
        logger.LogInformation($"[Event] User account deleted at {@event.Time}.\n\tInfo: {@event}");
        return;
    }   
}
