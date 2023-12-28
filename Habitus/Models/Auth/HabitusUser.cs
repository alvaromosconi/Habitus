using Microsoft.AspNetCore.Identity;

namespace Habitus.Models.Auth;

public class HabitusUser : IdentityUser
{
    public ICollection<Habit> UserHabits { get; set; }
}
