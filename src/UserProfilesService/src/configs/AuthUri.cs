
namespace Configs;

public class AuthUri
{
    public AuthUri(IConfiguration configuration)
    {
        var AuthEndpoint = configuration.GetValue<string>("Identity:AuthEndpoint");
        var clientId = configuration.GetValue<string>("Identity:ClientId");
        var redirectUri = configuration.GetValue<string>("Identity:RedirectUri");

        Get = new Uri($"{AuthEndpoint}" + $"?client_id={clientId}" 
                                     + $"&redirect_uri={redirectUri}" 
                                     + "&response_type=code");
    }
    
    public Uri Get {get; init;}
}
