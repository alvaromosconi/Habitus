using Habitus.Models;
using Habitus.Services.Communication;

namespace Habitus.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> ListAsync();
    Task AddAsync(Category category);
}
