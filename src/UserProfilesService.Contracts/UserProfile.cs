namespace Contracts;

public record UserProfile(
    string Id, 
    string Username, 
    string Email,
    DateTime? CreatedAt,
    Character[]? Characters
);
