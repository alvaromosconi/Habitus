using Microsoft.AspNetCore.Identity;

namespace Habitus.Domain.Models.Auth;

public class HabitusUser : IdentityUser
{
    public long ChatId { get;  set; }
    public ICollection<Habit> UserHabits { get; set; } = new List<Habit>();
}
