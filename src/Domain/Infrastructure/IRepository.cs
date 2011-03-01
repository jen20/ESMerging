using System;

namespace Domain
{
    public interface IRepository
    {
        TAggregateType GetById<TAggregateType>(Guid id) where TAggregateType : AggregateRootBase;
    }
}