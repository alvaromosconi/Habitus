using Habitus.Domain.Models;
using Habitus.Domain.Repositories;
using Habitus.Domain.Services;
using Habitus.Domain.Services.Communication;

namespace Habitus.Services;

public class HabitService : IHabitService
{
    private readonly IHabitRepository _habitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IReminderService _reminderService;

    public HabitService(IHabitRepository habitRepository,
                        ICategoryRepository categoryRepository,
                        IUnitOfWork unitOfWork,
                        IReminderService reminderService)
    {
        _habitRepository = habitRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _reminderService = reminderService;
    }
    public async Task<IEnumerable<Habit>> ListAsync()
    {
        return await _habitRepository.ListAsync();
    }

    public async Task<Response<Habit>> SaveAsync(Habit habit)
    {
        try
        {
            var existingCategory = await _categoryRepository.FindByIdAsync(habit.CategoryId);

            if (existingCategory == null)
            {
                return new Response<Habit>("Invalid category");
            }

            await _habitRepository.AddAsync(habit);
            await _unitOfWork.CompleteAsync();

            if (habit.NotifyByTelegram)
            {
                _reminderService.ScheduleReminder(habit);
            }

            return new Response<Habit>(habit);
        }
        catch (Exception ex)
        {
            return new Response<Habit>($"An error occurred when saving the habit: {ex.Message}");
        }
    }

    public async Task<Response<Habit>> UpdateAsync(int id, Habit habit)
    {
        var existingHabit = await _habitRepository.FindByIdAsync(id);

        if (existingHabit == null)
        {
            return new Response<Habit>("Habit not found.");
        }

        var existingCategory = await _categoryRepository.FindByIdAsync(habit.CategoryId);

        if (existingCategory == null)
        {
            return new Response<Habit>("Invalid category");
        }

        bool scheduledRemindersHasChanged = existingHabit.SelectedDays.Intersect(habit.SelectedDays).Any() || 
                                            existingHabit.NotificationTime != habit.NotificationTime;

        existingHabit.State = habit.State;
        existingHabit.NotificationTime = habit.NotificationTime;
        existingHabit.SelectedDays = habit.SelectedDays;
        existingHabit.Description = habit.Description;
        existingHabit.CategoryId = habit.CategoryId;
        existingHabit.Name = habit.Name;

        try
        {
            _habitRepository.Update(existingHabit);
            await _unitOfWork.CompleteAsync();

            if (scheduledRemindersHasChanged)
            {
                _reminderService.RemoveScheduledReminders(existingHabit);
                _reminderService.ScheduleReminder(existingHabit);
            }

            return new Response<Habit>(existingHabit);
        }
        catch (Exception ex)
        {
            return new Response<Habit>($"An error occurred when saving the habit: {ex.Message}");
        }
    }

    public async Task<Response<Habit>> DeleteAsync(int id)
    {
        var existingHabit = await _habitRepository.FindByIdAsync(id);

        if (existingHabit == null)
            return new Response<Habit>("Habit not found.");

        try
        {
            _habitRepository.Remove(existingHabit);
            await _unitOfWork.CompleteAsync();

            _reminderService.RemoveScheduledReminders(existingHabit);

            return new Response<Habit>(existingHabit);
        }
        catch (Exception ex)
        {
            return new Response<Habit>($"An error occurred when deleting the habit: {ex.Message}");
        }
    }

    public async Task<Response<Habit>> ToggleTelegramReminder(int id)
    {
        var existingHabit = await _habitRepository.FindByIdAsync(id);

        if (existingHabit == null)
        {
            return new Response<Habit>("Habit not found.");
        }

        if (existingHabit.NotifyByTelegram == false)
        {
            existingHabit.NotifyByTelegram = true;
            _reminderService.ScheduleReminder(existingHabit);
        }
        else
        {
            existingHabit.NotifyByTelegram = false;
            _reminderService.RemoveScheduledReminders(existingHabit);
        }

        try
        {
            _habitRepository.Update(existingHabit);
            await _unitOfWork.CompleteAsync();

            return new Response<Habit>(existingHabit);
        }
        catch (Exception ex)
        {
            return new Response<Habit>($"An error occurred when saving the habit: {ex.Message}");
        }
    }
}