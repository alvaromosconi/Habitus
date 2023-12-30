using Habitus.Domain.Models;
using Habitus.Domain.Models.Auth;

namespace Habitus.Resources;

public record HabitResource
{
    public int Id { get; init; }
    public HabitusUserResource User { get; init; }
    public CategoryResource Category { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public TimeSpan NotificationTime { get; init; }
    public HabitFrequency Frequency { get; init; }
    public HabitState State { get; init; }
}
