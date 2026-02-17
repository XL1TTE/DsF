
using Wolverine.Attributes;

namespace Events.User;

[WolverineMessage]
public record AccountDeleted
{
    public long Time { get; init; }
    public string RealmId { get; init; } = default!;
    
    public string ResourceType {get; init;} = default!;
    public string OperationType {get; init;} = default!;
    public string ResourcePath {get; init;} = default!;
    public string ResourceId {get; init;} = default!;
}
