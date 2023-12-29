using Habitus.Domain.Models;
using Habitus.Domain.Repositories;
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

    public async Task<Category> FindByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }
}
