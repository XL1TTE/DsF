using Commands;
using Events.User;
using Wolverine.Attributes;

namespace Handlers.OnAccountCreated;

/// <summary>
/// Handler for AccountRegistered event from Keycloak
/// Sends CreateProfile command with deserialized user data
/// </summary>
[WolverineHandler]
public class ValidateAndSendCreateProfile
{
    public CreateProfile Consume(AccountRegistered @event)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(@event.Representation.Username))
        {
            throw new ArgumentException(
                $"Keycloak event missing Username for user {@event.ResourceId}");
        }

        if (string.IsNullOrWhiteSpace(@event.Representation.Email))
        {
            throw new ArgumentException(
                $"Keycloak event missing Email for user {@event.ResourceId}");
        }

        return new CreateProfile(
            @event.ResourceId,
            @event.Representation.Username,
            @event.Representation.Email);
    }
}
