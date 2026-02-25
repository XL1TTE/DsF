
using Contracts;
using System.Text.Json.Serialization;

namespace Events.User;

/// <summary>
/// Keycloak event: User updated (USER.UPDATE)
/// Note: Representation comes as JSON string from Keycloak
/// </summary>
public record UserUpdated
(
    long Time,
    string RealmId,
    string ClientId,
    string ResourceId,
    string ResourcePath,
    string ResourceType,
    string OperationType,
    [property: JsonConverter(typeof(AccountRepresentationConverter))]
    AccountRepresentation Representation
);
