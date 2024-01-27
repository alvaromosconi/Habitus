using Habitus.Domain.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Habitus.Domain.Models;

public class Habit
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public virtual HabitusUser User { get; set; }
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<DayOfWeek> SelectedDays { get; set; }
    public TimeOnly NotificationTime { get; set; }
    public bool NotifyByTelegram { get; set; } = false;
    public HabitState State { get; set; }

};

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HabitState
{
    Active,
    Inactive
}



