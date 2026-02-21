using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using Shouldly;
using Testing.Fixtures;
using Wolverine;
using Events.User;
using Contracts;
using Documents;

namespace Testing.IntegrationTests;

/// <summary>
/// Integration tests for CreateProfile command chain
/// </summary>
[Collection("MongoDB Collection")]
public class create_profile_chain_tests(MongoDbFixture fixture) : IAsyncLifetime
{
    private IHost _host = null!;
    private IMessageBus _bus = null!;
    private MongoDbContext _context = null!;

    public async Task InitializeAsync()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Register MongoDB
                services.AddDbContext<MongoDbContext>(options =>
                {
                    options.UseMongoDB(fixture.ConnectionString, "testdb");
                });

                services.AddWolverine(opts =>
                {
                    opts.Discovery.IncludeAssembly(typeof(Handlers.OnAccountCreated.ValidateAndSendCreateProfile).Assembly);
                });
            }).Start();

        _bus = _host.Services.GetRequiredService<IMessageBus>();
        _context = _host.Services.GetRequiredService<MongoDbContext>();
    }

    public async Task DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }

    [Fact]
    public async Task account_registered_event_should_create_profile_in_mongodb()
    {
        // Arrange - Create AccountRegistered event (simulating Keycloak event)
        var userId = Guid.NewGuid().ToString();
        var representation = new AccountRepresentation
        {
            Username = "testuser",
            Email = "test@example.com"
        };
        var @event = new AccountRegistered(
            Time: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RealmId: "test-realm",
            ClientId: "test-client",
            ResourceId: userId,
            ResourcePath: $"users/{userId}",
            ResourceType: "USER",
            OperationType: "CREATE",
            Representation: representation
        );

        // Act - Send event through Wolverine
        await _bus.InvokeAsync(@event);

        // Assert - Verify profile was created in MongoDB
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AuthId == userId);
        profile.ShouldNotBeNull();
        profile.AuthId.ShouldBe(userId);
        profile.Username.ShouldBe("testuser");
        profile.Email.ShouldBe("test@example.com");
        profile.IsDeleted.ShouldBeFalse();
        profile.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task account_registered_event_with_existing_profile_should_not_create_duplicate()
    {
        // Arrange - Create existing profile
        var userId = Guid.NewGuid().ToString();
        var existingProfile = new ProfileDocument
        {
            Id = Guid.NewGuid().ToString(),
            AuthId = userId,
            Username = "existinguser",
            Email = "existing@example.com",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.Profiles.AddAsync(existingProfile);
        await _context.SaveChangesAsync();

        // Act - Send AccountRegistered event
        var representation = new AccountRepresentation
        {
            Username = "newuser",
            Email = "new@example.com"
        };
        var @event = new AccountRegistered(
            Time: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RealmId: "test-realm",
            ClientId: "test-client",
            ResourceId: userId,
            ResourcePath: $"users/{userId}",
            ResourceType: "USER",
            OperationType: "CREATE",
            Representation: representation
        );
        await _bus.InvokeAsync(@event);

        // Assert - Verify no duplicate was created
        var profiles = await _context.Profiles.Where(p => p.AuthId == userId).ToListAsync();
        profiles.Count.ShouldBe(1);
        var profile = profiles[0];
        profile.Username.ShouldBe("existinguser"); // Original profile unchanged
        profile.Email.ShouldBe("existing@example.com");
    }

    [Fact]
    public async Task account_registered_event_with_missing_username_should_not_create_profile()
    {
        // Arrange - Event with missing username
        var userId = Guid.NewGuid().ToString();
        var representation = new AccountRepresentation
        {
            Username = null!,
            Email = "test@example.com"
        };
        var @event = new AccountRegistered(
            Time: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RealmId: "test-realm",
            ClientId: "test-client",
            ResourceId: userId,
            ResourcePath: $"users/{userId}",
            ResourceType: "USER",
            OperationType: "CREATE",
            Representation: representation
        );

        // Act - Send event (handler will throw but InvokeAsync won't propagate)
        await _bus.InvokeAsync(@event).ShouldThrowAsync<ArgumentException>();

        // Assert - Verify no profile was created
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AuthId == userId);
        profile.ShouldBeNull();
    }

    [Fact]
    public async Task account_registered_event_with_missing_email_should_not_create_profile()
    {
        // Arrange - Event with missing email
        var userId = Guid.NewGuid().ToString();
        var representation = new AccountRepresentation
        {
            Username = "testuser",
            Email = null!
        };
        var @event = new AccountRegistered(
            Time: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RealmId: "test-realm",
            ClientId: "test-client",
            ResourceId: userId,
            ResourcePath: $"users/{userId}",
            ResourceType: "USER",
            OperationType: "CREATE",
            Representation: representation
        );

        // Act - Send event
        await _bus.InvokeAsync(@event).ShouldThrowAsync<ArgumentException>();

        // Assert - Verify no profile was created
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AuthId == userId);
        profile.ShouldBeNull();
    }
}
