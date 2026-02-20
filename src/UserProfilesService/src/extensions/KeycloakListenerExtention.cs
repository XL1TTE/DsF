using Configs;
using Events.User;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Extensions;


internal class KeycloakExtentions: IWolverineExtension
{
    public void Configure(WolverineOptions options)
    {
        options.Services.TryAddSingleton<AuthUri>();

        options.ListenToRabbitQueue("Events.User.AccountRegistered")
               .DefaultIncomingMessage<AccountRegistered>();

        options.ListenToRabbitQueue("Events.User.AccountDeleted")
               .DefaultIncomingMessage<AccountDeleted>();
    }
}

