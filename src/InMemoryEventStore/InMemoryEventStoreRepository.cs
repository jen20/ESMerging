using System;
using Domain;

namespace InMemoryEventStore
{
    public class InMemoryEventStoreRepository : IRepository
    {
        private readonly IEventStore _eventStore;

        public InMemoryEventStoreRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        
        public TAggregateType GetById<TAggregateType>(Guid id) where TAggregateType : AggregateRootBase
        {
            var events = _eventStore.GetEventsForAggregate(id);
            var aggregate = (TAggregateType)Activator.CreateInstance(typeof (TAggregateType), true);
            aggregate.LoadFromEventStream(events);

            return aggregate;
        }
    }
}