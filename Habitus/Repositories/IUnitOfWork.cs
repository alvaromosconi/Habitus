namespace Habitus.Repositories;

public interface IUnitOfWork
{
    Task CompleteAsync();
}