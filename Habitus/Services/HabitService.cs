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
        throw new NotImplementedException();
    }

    public async Task<Response<Habit>> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
