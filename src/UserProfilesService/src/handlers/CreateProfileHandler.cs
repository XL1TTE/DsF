using Commands;
using Contracts;
using Documents;
using Events;
using MongoDB.Bson;
using Persistence;
using Wolverine;
using Wolverine.Attributes;

namespace Handlers;

public class CreateProfileHandler
{
    [WolverineHandler]
    public async Task Consume(CreateProfile command, MongoDbContext db, ILogger<Loggers.ProfileEvents> logger)
    {
        ProfileDocument? profileExist = db.Profiles.FirstOrDefault(x => x.AuthId == command.userId);
        if (profileExist != null)
        {
            logger.LogInformation($"[{nameof(CreateProfileHandler)}] Profile for user: {command.userId} already exist.");
        }

        var profile = new ProfileDocument
        {
            Id = Guid.NewGuid().ToString(),
            AuthId = command.userId,
            Email = command.email,
            Username = command.username,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            Characters = []
        };

        await db.Profiles.AddAsync(profile);
        await db.SaveChangesAsync();

        logger.LogInformation($"[{nameof(CreateProfileHandler)}] Profile for user created:\n\t\t Preview: {profile.ToJson()}");
    }
}
