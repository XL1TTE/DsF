using Wolverine;

namespace Handlers;

public static class UserProfileCreatedHandler
{
    public static async Task Consume(UserProfileCreated message)
    {
        Console.WriteLine($"User profile created: {message}.");
    }   
}
