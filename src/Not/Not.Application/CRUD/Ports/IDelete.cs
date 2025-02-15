using Not.Domain;
using Not.Injection;

namespace Not.Application.CRUD.Ports;

public interface IDelete<T> : ITransient
    where T : IAggregateRoot
{
    Task Delete(int id);
    Task Delete(T entity);
    Task Delete(Predicate<T> filter);
    Task Delete(IEnumerable<T> entities);
}
