

using Events.User;
using Wolverine.Attributes;

namespace Handlers;

[WolverineHandler]
public static class AccountDeletedHandler {
    
    public static async Task Consume(AccountDeleted @event)
    {
        Console.WriteLine($"Account deleted: {@event}");
        return;
    }   
}
