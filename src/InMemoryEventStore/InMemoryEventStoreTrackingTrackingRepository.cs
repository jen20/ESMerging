using System;
using Domain;

namespace InMemoryEventStore
{
    public class InMemoryEventStoreTrackingRepository<TAggregateType> : ITrackingRepository<TAggregateType> where TAggregateType : AggregateRootBase
    {
        private readonly IEventStore _eventStore;
        private TAggregateType _trackedAggregate;

        

        public InMemoryEventStoreTrackingRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public TAggregateType GetByIdAndTrack(Guid aggregateId)
        {
            _trackedAggregate = GetById(aggregateId);
            return _trackedAggregate;
        }

        public TAggregateType TrackedAggregate
        {
            get { return _trackedAggregate; }
        }

        public void CommitTrackedAggregate()
        {
            if (!_trackedAggregate.HasChanges())
                return;

            _eventStore.SaveEvents(_trackedAggregate.AggregateId, _trackedAggregate.GetUncommittedChanges(),
                                   _trackedAggregate.AggregateVersion);

            _trackedAggregate = null;
        }

        public void TrackNewAggregate(TAggregateType aggregate)
        {
            _trackedAggregate = aggregate;
        }
        
        public TAggregateType GetById(Guid aggregateId)
        {
            var aggregateEvents = _eventStore.GetEventsForAggregate(aggregateId);

            var aggregate = (TAggregateType)Activator.CreateInstance(typeof(TAggregateType), true);
            aggregate.LoadFromEventStream(aggregateEvents);
            return aggregate;
        }
    }
}