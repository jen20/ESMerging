using System;

namespace Domain
{
    public interface IRepository<T> where T : AggregateRootBase
    {
        bool TryGetById(Guid id, out T aggregate);
    }
}