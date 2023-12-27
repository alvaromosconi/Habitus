using Habitus.Models;
using Microsoft.EntityFrameworkCore;
public class HabitusContext : DbContext
{
    public HabitusContext(DbContextOptions options): base(options)
    {
    }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitusUser> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
}