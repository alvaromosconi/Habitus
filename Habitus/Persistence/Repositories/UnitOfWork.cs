using Habitus.Domain.Repositories;

namespace Habitus.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HabitusContext _context;

    public UnitOfWork(HabitusContext context)
    {
        _context = context;
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }
}