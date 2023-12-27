namespace Habitus.Models;

public class Habit
{
    public int Id { get; set; }

    public string UserID { get; set; }

    public HabitusUser User;

    public Category Category;

    public int CategoryId { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }
    
    public TimeSpan NotificationTime { get; set; }

    public HabitFrequency Frequency { get; set; } = HabitFrequency.Daily;

    public HabitState State { get; set; } = HabitState.Active;
}

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

