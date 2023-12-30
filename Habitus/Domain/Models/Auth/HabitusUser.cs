using Microsoft.AspNetCore.Identity;

namespace Habitus.Domain.Models.Auth;

public class HabitusUser : IdentityUser
{
    public virtual ICollection<Habit>? UserHabits { get; set; }
}
