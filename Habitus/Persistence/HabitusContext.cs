using Habitus.Domain.Models;
using Habitus.Domain.Models.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
public class HabitusContext : IdentityDbContext<HabitusUser>
{
    public HabitusContext(DbContextOptions options): base(options)
    {
    }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitusUser> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
}