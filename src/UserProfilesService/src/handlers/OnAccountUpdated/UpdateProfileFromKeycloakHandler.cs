using Events.User;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Wolverine.Attributes;

namespace Handlers.OnAccountUpdated;

/// <summary>
/// Handles Keycloak USER.UPDATE event from RabbitMQ.
/// Updates user profile in MongoDB when Keycloak sends update notification.
/// </summary>
[WolverineHandler]
public class UpdateProfileFromKeycloakHandler
{
    public async Task Consume(UserUpdated @event, MongoDbContext db, ILogger<UpdateProfileFromKeycloakHandler> logger)
    {
        var representation = @event.Representation;

        var keycloakUserId = @event.ResourceId;

        logger.LogInformation("Received UserUpdated event for UserId: {UserId}, ResourcePath: {Path}", keycloakUserId, @event.ResourcePath);

        var profile = await db.Profiles.FirstOrDefaultAsync(x => x.UserId == keycloakUserId);

        if (profile == null)
        {
            logger.LogWarning("Profile not found for UserId: {UserId}", keycloakUserId);
            return;
        }

        if (representation.Username != null && representation.Username != profile.Username)
            profile.Username = representation.Username;

        if (representation.Email != null && representation.Email != profile.Email)
            profile.Email = representation.Email;

        await db.SaveChangesAsync();
        
        logger.LogInformation("Profile updated for UserId: {UserId}", keycloakUserId);
    }
}
