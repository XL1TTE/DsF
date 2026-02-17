

using Events.User;
using Wolverine.Attributes;

namespace Handlers;

[WolverineHandler]
public static class AccountRegisteredHandler {
    
    public static async Task Consume(AccountRegistered @event)
    {
        Console.WriteLine($"Account registered: {@event}");
        return;
    }   
}
