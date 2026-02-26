
namespace Contracts.Responses;

public class RaceDto
{
    public string Name { get; set; } = default!;
    public string History { get; set; } = default!;
    public int Health { get; set; }
    public string? PreviewImagePath { get; set; }

    public RaceDto(string name, string history, int health, string? previewImagePath = null)
    {
        Name = name;
        History = history;
        Health = health;
        PreviewImagePath = previewImagePath;
    }
    public RaceDto() { }
}
