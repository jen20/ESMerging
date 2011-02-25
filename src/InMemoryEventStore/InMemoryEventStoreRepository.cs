using System;
using System.Collections.Generic;
using Domain;
using Events;

namespace InMemoryEventStore
{
    public class InMemoryEventStoreRepository<TAggregateType> : IRepository<TAggregateType> where TAggregateType : AggregateRootBase
    {
        private readonly IUnitOfWork<TAggregateType> _unitOfWork;

        public InMemoryEventStoreRepository(IUnitOfWork<TAggregateType> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool TryGetById(Guid aggregateId, out TAggregateType aggregate)
        {
            return _unitOfWork.TryGetById(aggregateId, out aggregate);
        }
    }

    public interface IUnitOfWork<TAggregateType> where TAggregateType : AggregateRootBase
    {
        bool TryGetById(Guid aggregateId, out TAggregateType aggregate);
       // void AddNewAggregate(TAggregateType aggregate);
       // void Commit();
    }

    public class EventStoreUnitOfWork<TAggregateType> : IUnitOfWork<TAggregateType> where TAggregateType : AggregateRootBase
    {
        private readonly IEventStore _eventStore;
        private readonly List<TAggregateType> _trackedAggregates;

        public EventStoreUnitOfWork(IEventStore eventStore)
        {
            _trackedAggregates = new List<TAggregateType>();
            _eventStore = eventStore;
        }

        public bool TryGetById(Guid aggregateId, out TAggregateType aggregate)
        {
            IEnumerable<Event> aggregateEvents;

            try
            {
                aggregateEvents = _eventStore.GetEventsForAggregate(aggregateId);
            } catch (AggregateNotFoundException)
            {
                aggregate = null;
                return false;
            }

            aggregate = (TAggregateType) Activator.CreateInstance(typeof (TAggregateType), true);
            aggregate.LoadFromEventStream(aggregateEvents);

            _trackedAggregates.Add(aggregate);

            return true;
        }
    }
}