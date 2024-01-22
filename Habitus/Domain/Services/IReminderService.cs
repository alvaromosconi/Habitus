using Habitus.Domain.Models;

namespace Habitus.Domain.Services;

public interface IReminderService
{
    Task ScheduleReminder(Habit habit);
}
