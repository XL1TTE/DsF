namespace Commands;

public record GetAllRaces(
    int Skip = 0,
    int Take = 100
);
