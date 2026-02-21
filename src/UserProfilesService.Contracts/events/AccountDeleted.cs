
namespace Events.User;

/// <summary>
/// Model of Keycloak event
/// </summary>
public record AccountDeleted
(
    long Time,
    string RealmId,
    string ResourceType,
    string OperationType,
    string ResourcePath,
    string ResourceId
);
