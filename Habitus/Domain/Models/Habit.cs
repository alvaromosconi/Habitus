using Habitus.Domain.Models.Auth;

namespace Habitus.Domain.Models;

public record Habit
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public virtual HabitusUser User { get; set; }
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public TimeSpan NotificationTime { get; set; }
    public HabitFrequency Frequency { get; set; }
    public HabitState State { get; set; }
};

public enum HabitFrequency
{
    Minutely,
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Custom
}

public enum HabitState
{
    Active,
    Inactive
}

