using System.ComponentModel.DataAnnotations;

namespace Habitus.Resources;

public record SaveCategoryResource
{
    [Required]
    [MaxLength(30)]
    public string? Name { get; init; }
}