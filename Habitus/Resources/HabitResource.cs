using Habitus.Domain.Models;
using Habitus.Domain.Models.Auth;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace Habitus.Resources;

public record HabitResource
{
    public int Id { get; init; }
    public HabitusUserResource User { get; init; }
    public CategoryResource Category { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public TimeOnly NotificationTime { get; init; }
    public List<DayOfWeek> SelectedDays { get; init; }
    public HabitState State { get; init; }
}
