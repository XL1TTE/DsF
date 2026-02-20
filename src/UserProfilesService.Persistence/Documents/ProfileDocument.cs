using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace Documents;

[Collection("user-profiles")]
public sealed class ProfileDocument
{
    [BsonId] public required string Id { get; init; }
    [EmailAddress] public required string Email { get; set; }
    [BsonElement] public required string Username { get; set; }
    [BsonElement] public required DateTime CreatedAt { get; init; }
    [BsonElement] public CharacterDocument[]? Characters { get; init; }
}
