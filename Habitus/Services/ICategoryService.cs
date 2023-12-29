using Habitus.Models;
using Habitus.Services.Communication;

namespace Habitus.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> ListAsync();
    Task<CategoryResponse> SaveAsync(Category category);
    Task<CategoryResponse> UpdateAsync(int id, Category category);
    Task<CategoryResponse> DeleteAsync(int id);
}
