using Wolverine.Attributes;

namespace Messages;

public record CreateUserProfile(
    string username,
    string email
);
