using Habitus.Domain.Models;
using Habitus.Domain.Models.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        var dayOfWeekConverter = new ValueConverter<DayOfWeek, string>(
            v => v.ToString(),
            s => Enum.Parse<DayOfWeek>(s)
        );

        var listDayOfWeekConverter = new ValueConverter<List<DayOfWeek>, string>(
            v => string.Join(", ", v.Select(d => d.ToString())),
            s => s.Split(", ", StringSplitOptions.RemoveEmptyEntries)
                  .Select(d => Enum.Parse<DayOfWeek>(d)).ToList()
        );

        modelBuilder.Entity<Habit>()
            .Property(h => h.SelectedDays)
            .HasConversion(listDayOfWeekConverter);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Health" },
            new Category { Id = 2, Name = "Financial" },
            new Category { Id = 3, Name = "Personal Development" },
            new Category { Id = 4, Name = "Hobbies " }
        );
    }

}