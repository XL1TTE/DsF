
using System.Text.Json.Serialization;

namespace Contracts;

/// <summary>
/// Keycloak Account Representation from event payload
/// </summary>
public class AccountRepresentation
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
    
    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }
}
