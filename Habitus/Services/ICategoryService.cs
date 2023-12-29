using Habitus.Models;
using Habitus.Services.Communication;

namespace Habitus.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> ListAsync();
    Task<SaveCategoryResponse> SaveAsync(Category category);
}
