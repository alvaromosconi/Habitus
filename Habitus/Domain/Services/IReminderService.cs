using Habitus.Domain.Models;

namespace Habitus.Domain.Services;

public interface IReminderService
{
    void RemoveScheduledReminders(Habit habit);
    void ScheduleReminder(Habit habit);
}
