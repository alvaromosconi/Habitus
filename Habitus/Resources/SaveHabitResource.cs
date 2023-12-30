using Habitus.Domain.Models;
using System.ComponentModel.DataAnnotations;
namespace Habitus.Resources;

public record SaveHabitResource
{
    [Required]
    public int CategoryId { get; init; }
    [Required]
    [StringLength(30)]
    public string Name { get; init; }
    [StringLength(150)]
    [DataType(DataType.MultilineText)]
    public string? Description { get; init; }
    [Required]
    [DataType(DataType.Time)]
    public TimeSpan NotificationTime { get; init; }
    [Required]
    public HabitFrequency Frequency { get; init; }
    public HabitState State { get; init; } = HabitState.Active;
}