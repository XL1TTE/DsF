using Events;
using Loggers;
using Wolverine.Attributes;

namespace Handlers.OnProfileCreated;

[WolverineHandler]
public class LogEvent
{
    public async Task Consume(ProfileCreated @event, ILogger<ProfileEvents> logger)
    {
        logger.LogInformation($"[Event] New user account profile created at {@event.at}.\n\tInfo: {@event}");
    }
}
