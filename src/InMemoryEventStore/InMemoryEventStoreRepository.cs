using System;
using Domain;

namespace InMemoryEventStore
{
    public class InMemoryEventStoreRepository<T> : IRepository<T> where T : AggregateRootBase
    {
        private readonly IEventStore _eventStore;

        public InMemoryEventStoreRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        #region Implementation of IRepository<T>

        public void Save(T aggregate, int expectedVersion)
        {
            _eventStore.SaveEvents(aggregate.AggregateId, aggregate.GetUncommittedChanges(), expectedVersion);
        }

        public T GetById(Guid id)
        {
            var aggregate = (T)Activator.CreateInstance(typeof (T), true);
            aggregate.LoadFromEventStream(_eventStore.GetEventsForAggregate(id));
            return aggregate;
        }

        #endregion
    }
}