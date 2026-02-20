
using Wolverine.Attributes;

namespace Events.User;

[WolverineMessage]
public record AccountRegistered
{
    public long Time { get; init; }
    public string RealmId { get; init; } = default!;
    public string ClientId { get; init; } = default!;
    public string ResourceId { get; set; } = default!;
    public string ResourcePath { get; set; } = default!;
    public string ResourceType { get; set; } = default!;
    public string OperationType { get; init; } = default!;
    public string Representation { get; init; } = default!;
    
}
