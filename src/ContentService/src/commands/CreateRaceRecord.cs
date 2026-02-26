namespace ContentService.Commands;

public record CreateRaceRecord(
    string Name,
    string History,
    int Health,
    string PreviewUrl
);
