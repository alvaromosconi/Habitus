using Habitus.Domain.Models;
using Habitus.Domain.Services;

namespace Habitus.Services;

public class ReminderService : IReminderService
{

    private readonly TelegramService _telegramService;

    public ReminderService(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }
    public async Task ScheduleReminder(Habit habit)
    {
        string message = $"¡It's time for your habit: {habit.Name}!\n\n{habit.Description}";

        try
        {
            await _telegramService.ScheduleMessage(habit.NotificationTime.Hour,
                                                   habit.NotificationTime.Minute,
                                                   message,
                                                   habit.User.ChatId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }
    
}
