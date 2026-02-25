using Commands;
using Configs;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;

namespace Handlers;

[WolverineHandler]
public class UpdateProfileHandler
{
    /// <summary>
    /// Returns redirect URL to Keycloak account edit page.
    /// After user updates profile in Keycloak, an event will be sent to RabbitMQ
    /// and handled by OnUserUpdated handler to sync MongoDB.
    /// </summary>
    public ActionResult Consume(UpdateProfile command, AuthUri authUri)
    {
        // Redirect to Keycloak Account Console for profile editing
        // Keycloak will send USER.UPDATE event to RabbitMQ after successful update
        var keycloakAccountUrl = $"{authUri.Get.Scheme}://{authUri.Get.Authority}/realms/DsF/account";
        
        return new RedirectResult(keycloakAccountUrl);
    }
}
