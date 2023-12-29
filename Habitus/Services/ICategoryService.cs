using Habitus.Models;

namespace Habitus.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> ListAsync();
}
