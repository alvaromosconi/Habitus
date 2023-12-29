namespace Habitus.Domain.Repositories;

public interface IUnitOfWork
{
    Task CompleteAsync();
}