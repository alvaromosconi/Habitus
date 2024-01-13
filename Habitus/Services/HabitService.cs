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

    public HabitService(IHabitRepository habitRepository,
                        ICategoryRepository categoryRepository, 
                        IUnitOfWork unitOfWork)
    {
        _habitRepository = habitRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
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

            return new Response<Habit>(existingHabit);
        }
        catch (Exception ex)
        {
            return new Response<Habit>($"An error occurred when saving the category: {ex.Message}");
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

            return new Response<Habit>(existingHabit);
        }
        catch (Exception ex)
        {
            return new Response<Habit>($"An error occurred when deleting the habit: {ex.Message}");
        }
    }
}
