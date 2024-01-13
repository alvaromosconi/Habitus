using Habitus.Domain.Models;
using Habitus.Domain.Repositories;
using Habitus.Domain.Services;
using Habitus.Domain.Services.Communication;

namespace Habitus.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository,
                           IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;   
    }

    public async Task<IEnumerable<Category>> ListAsync()
    {
        return await _categoryRepository.ListAsync();
    }

    public async Task<Response<Category>> SaveAsync(Category category)
    {
        try
        {
            await _categoryRepository.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return new Response<Category>(category);
        }
        catch (Exception ex)
        {
            return new Response<Category>($"An error occurred when saving the category: {ex.Message}");
        }
    }

    public async Task<Response<Category>> UpdateAsync(int id, Category category)
    {
        var existingCategory = await _categoryRepository.FindByIdAsync(id);

        if (existingCategory == null)
        {
            return new Response<Category>("Category not found.");
        }

        existingCategory.Name = category.Name;

        try
        {
            _categoryRepository.Update(existingCategory);
            await _unitOfWork.CompleteAsync();

            return new Response<Category>(existingCategory);
        }
        catch (Exception ex)
        {
            return new Response<Category>($"An error occurred when saving the category: {ex.Message}");
        }
    }

    public async Task<Response<Category>> DeleteAsync(int id)
    {
        var existingCategory = await _categoryRepository.FindByIdAsync(id);

        if (existingCategory == null)
            return new Response<Category>("Category not found.");

        try
        {
            _categoryRepository.Remove(existingCategory);
            await _unitOfWork.CompleteAsync();

            return new Response<Category>(existingCategory);
        }
        catch (Exception ex)
        {
            return new Response<Category>($"An error occurred when deleting the category: {ex.Message}");
        }
    }

}
