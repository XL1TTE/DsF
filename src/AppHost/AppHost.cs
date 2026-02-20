using Extenstions;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var MessagingLogin    = builder.AddParameter("messaging-login", builder.FromConfig<string>("Messaging:Login") ?? "admin");
var MessagingPassword = builder.AddParameter("messaging-password", builder.FromConfig<string>("Messaging:Password") ?? "admin");

var Auth = builder.AddKeycloak("auth-provider", port: 8080).WithDataVolume();
var Messaging            = builder.AddRabbitMQ("messaging").WithManagementPlugin().WithDataVolume();
var UsersDataBase        = builder.AddMongoDB("users-db");

var UserProfileService = builder
                         .AddProject<UserProfilesService>("user-profiles")
                         .WithReference(UsersDataBase)
                         .WithReference(Auth)
                         .WithReference(Messaging)
                         .WaitFor(UsersDataBase);

var MessagingEndpoint = Messaging.GetEndpoint("tcp");

Auth.WithEnvironment(    "KK_TO_RMQ_URL",      MessagingEndpoint.Property(EndpointProperty.Host))
        .WithEnvironment("KK_TO_RMQ_PORT",     MessagingEndpoint.Property(EndpointProperty.Port))
        .WithEnvironment("KK_TO_RMQ_VHOST",    "/")
        .WithEnvironment("KK_TO_RMQ_USERNAME", MessagingLogin)
        .WithEnvironment("KK_TO_RMQ_PASSWORD", MessagingPassword)
        .WithEnvironment("KK_TO_RMQ_EXCHANGE", "Auth.Events");


var KeyCloakPlugins = builder.FromConfig<string>("AuthService:Plugins");
if (KeyCloakPlugins != null) {
    Directory.GetFiles(KeyCloakPlugins, "*.jar").ToList().ForEach(plugin =>
    {
        Console.WriteLine($"Keycloak: {Path.GetFileName(plugin)} plugin mount.");
    });
    Auth.WithBindMount(KeyCloakPlugins, "/opt/keycloak/providers");
}

builder.Build().Run();
