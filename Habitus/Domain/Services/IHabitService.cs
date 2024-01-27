using Habitus.Domain.Models;
using Habitus.Domain.Services.Communication;

namespace Habitus.Domain.Services;

public interface IHabitService
{
    Task<IEnumerable<Habit>> ListAsync();
    Task<Response<Habit>> SaveAsync(Habit habit);
    Task<Response<Habit>> UpdateAsync(int id, Habit habit);
    Task<Response<Habit>> DeleteAsync(int id);
    Task<Response<Habit>> ToggleTelegramReminder(int id);
}