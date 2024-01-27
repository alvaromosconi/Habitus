using Habitus.Domain.Models;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;

namespace Habitus.Requests;

/// <summary>
/// Represents the data required to create or update a habit.
/// </summary>
public record SaveHabitRequest
{
    /// <summary>
    /// The unique identifier of the category.
    /// </summary>
    [Required]
    public int CategoryId { get; init; }

    /// <summary>
    /// The habit name.
    /// </summary>
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "The habit name must be between 3 and 30 characters.")]
    public string Name { get; init; }

    /// <summary>
    /// The habit description.
    /// </summary>
    [StringLength(150, ErrorMessage = "The habit description cannot exceed 150 characters.")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; init; }

    /// <summary>
    /// The list of days when the habit occurs.
    /// </summary>
    [Required]
    public List<string> SelectedDays { get; init; }

    /// <summary>
    /// The specific time during the day when the notification appears
    /// </summary>
    [Required]
    [DataType(DataType.Time)]
    public TimeOnly NotificationTime { get; init; }

    /// <summary>
    /// The state of the habit (Active, Inactive, etc.).
    /// </summary>
    public HabitState State { get; init; } = HabitState.Active;
}

public class SaveHabitRequestExample : IExamplesProvider<SaveHabitRequest>
{
    public SaveHabitRequest GetExamples()
    {
        return new SaveHabitRequest
        {
            CategoryId = 1,
            Name = "Morning Exercise",
            Description = "Daily morning workout routine",
            SelectedDays = new List<string> { "asdasd"},
            NotificationTime = new TimeOnly(08,00),
            State = HabitState.Active
        };
    }
}
