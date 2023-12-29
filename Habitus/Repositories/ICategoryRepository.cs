using Habitus.Models;

namespace Habitus.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> ListAsync();
}
