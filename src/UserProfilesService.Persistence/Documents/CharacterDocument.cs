using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace Documents;

[Collection("characters")]
public sealed class CharacterDocument
{
    [BsonId] public required string Id { get; init; }
    [BsonElement] public required string Name { get; set; }
    [BsonElement] public required string Race { get; init; }
}
