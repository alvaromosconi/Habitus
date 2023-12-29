using Habitus.Models;
using Habitus.Repositories;
using Habitus.Services.Communication;

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

    public async Task<SaveCategoryResponse> SaveAsync(Category category)
    {
        try
        {
            await _categoryRepository.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return new SaveCategoryResponse(category);
        }
        catch (Exception ex)
        {
            return new SaveCategoryResponse($"An error occurred when saving the category: {ex.Message}");
        }
    }
}
