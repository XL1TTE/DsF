namespace Events;

public record struct ProfileCreated(
    string id,
    DateTime at
);
