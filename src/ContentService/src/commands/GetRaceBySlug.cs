
using System.ComponentModel.DataAnnotations;

namespace Commands;

public record GetRaceBySlug(
    [MinLength(3)] string slug
);
