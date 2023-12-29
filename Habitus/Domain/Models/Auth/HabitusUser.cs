﻿using Habitus.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Habitus.Domain.Models.Auth;

public class HabitusUser : IdentityUser
{
    public ICollection<Habit>? UserHabits { get; set; }
}
