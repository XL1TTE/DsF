
using Common.DataAccess;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Documents;

public class RaceDocument : IDbEntity
{
    [BsonId]
    public string? Id {get; set;}

    [BsonElement]
    public string Slug {get; set; } = default!;

    [BsonElement]
    public string Name {get; set; } = default!;

    [BsonElement]
    public string History { get; set; } = default!;

    [BsonElement]
    public int Health { get; set; } = default!;

    [BsonElement]
    public string PreviewUrl { get; set; } = default!;
}
