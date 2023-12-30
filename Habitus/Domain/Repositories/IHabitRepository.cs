using Habitus.Domain.Models;

namespace Habitus.Domain.Repositories;

public interface IHabitRepository
{
    Task AddAsync(Habit habit);
    Task<Habit> FindByIdAsync(int id);
    Task<IEnumerable<Habit>> ListAsync();
    void Remove(Habit habit);
    void Update(Habit habit);
}