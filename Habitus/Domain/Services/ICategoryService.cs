using Habitus.Domain.Models;
using Habitus.Domain.Services.Communication;

namespace Habitus.Domain.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> ListAsync();
    Task<Response<Category>> SaveAsync(Category category);
    Task<Response<Category>> UpdateAsync(int id, Category category);
    Task<Response<Category>> DeleteAsync(int id);
}
