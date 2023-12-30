using Habitus.Domain.Models;
using Habitus.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Habitus.Persistence.Repositories;

public class HabitRepository : IHabitRepository
{
    private readonly HabitusContext _context;
    public HabitRepository(HabitusContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Habit>> ListAsync()
    {
        return await _context.Habits
                             .Include(h => h.Category)
                             .Include(h => h.User)
                             .ToListAsync();
    }

    public async Task AddAsync(Habit habit)
    {
        await _context.Habits.AddAsync(habit);
    }

    public async Task<Habit> FindByIdAsync(int id)
    {
        return await _context.Habits
                             .Include(h => h.Category)
                             .Include(h => h.User)
                             .FirstOrDefaultAsync(h => h.Id == id);
    }

    public void Update(Habit habit)
    {
        _context.Habits.Update(habit);
    }

    public void Remove(Habit habit)
    {
        _context.Habits.Remove(habit);
    }
}
