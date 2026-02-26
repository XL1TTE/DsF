
using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests;

public record CreateRaceRequest(
    [MinLength(3)]
    string Name,
    string History,
    [Range(0, 30)]
    int Health,
    IFormFile Preview
);
