using Habitus.Models;
using Habitus.Services.Communication;
using Microsoft.EntityFrameworkCore;

namespace Habitus.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly HabitusContext _context;
    public CategoryRepository(HabitusContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> ListAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }
}
