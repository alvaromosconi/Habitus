using Microsoft.AspNetCore.Identity;

namespace Habitus.Models;

public class HabitusUser : IdentityUser
{
    public ICollection<Habit> UserHabits { get; set; }
}
