using Habitus.Domain.Models;
using Habitus.Domain.Services;
using Hangfire;

namespace Habitus.Services;

public class ReminderService : IReminderService
{
    private readonly TelegramService _telegramService;

    public ReminderService(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }
    public void ScheduleReminder(Habit habit)
    {
        string message = $"¡It's time for your habit!: {habit.Name}\n\n{habit.Description}";

        try
        {
            foreach (var day in habit.SelectedDays)
            {
                DateTime nextReminder = CalculateNextReminderDateTime(day, habit.NotificationTime);
                string jobKey = $"habit_{habit.Id}_day_{day}";
                RecurringJob.AddOrUpdate(jobKey,
                                         () => _telegramService.ScheduleMessage(message, habit.User.ChatId),
                                         Cron.Weekly(day, nextReminder.Hour, nextReminder.Minute));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }

    private DateTime CalculateNextReminderDateTime(DayOfWeek selectedDay, TimeOnly notificationTime)
    {
        DateTime now = DateTime.UtcNow;
        DayOfWeek currentDay = now.DayOfWeek;

        int daysUntilNext = ((int)selectedDay - (int)currentDay + 7) % 7;
        DateTime nextReminder = now.AddDays(daysUntilNext);

        nextReminder = new DateTime(nextReminder.Year, nextReminder.Month, nextReminder.Day,
                                     notificationTime.Hour, notificationTime.Minute, 0);

        return nextReminder;
    }
    public void RemoveScheduledReminders(Habit habit)
    {
        if (habit.NotifyByTelegram)
        {
            try
            {
                foreach (var day in habit.SelectedDays)
                {
                    string jobKey = $"habit_{habit.Id}_day_{day}";
                    RecurringJob.RemoveIfExists(jobKey);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
        
    }
}
    

