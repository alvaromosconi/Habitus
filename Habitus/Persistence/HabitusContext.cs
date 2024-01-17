using Habitus.Domain.Models;
using Habitus.Domain.Models.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class HabitusContext : IdentityDbContext<HabitusUser>
{
    public HabitusContext(DbContextOptions options): base(options)
    {
    }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitusUser> Users { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var dayOfWeekConverter = new ValueConverter<List<DayOfWeek>, string>(
            v => string.Join(", ", v),
            s => s.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                  .Select(dayStr => Enum.Parse<DayOfWeek>(dayStr))
                  .ToList()
        );

        modelBuilder.Entity<Habit>()
            .Property(e => e.SelectedDays)
            .HasConversion(dayOfWeekConverter);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Health" },
            new Category { Id = 2, Name = "Financial" },
            new Category { Id = 3, Name = "Personal Development" },
            new Category { Id = 4, Name = "Hobbies " }
        );
    }

}