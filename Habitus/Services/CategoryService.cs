using Habitus.Models;
using Habitus.Repositories;

namespace Habitus.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<IEnumerable<Category>> ListAsync()
    {
        return await _categoryRepository.ListAsync();
    }
}
