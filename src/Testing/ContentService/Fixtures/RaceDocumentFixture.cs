using Persistence.Documents;

namespace Testing.ContentService.Fixtures;

public class RaceDocumentFixture
{
    public RaceDocument Create(
        string? slug = null,
        string? name = null,
        string? history = null,
        int health = 100,
        string? previewUrl = null)
    {
        return new RaceDocument
        {
            Slug = slug ?? $"race-{Guid.NewGuid():N}",
            Name = name ?? "Test Race",
            History = history ?? "Test History",
            Health = health,
            PreviewUrl = previewUrl ?? "/uploads/default.jpg"
        };
    }
}
