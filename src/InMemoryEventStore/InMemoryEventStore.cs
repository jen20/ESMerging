using System;
using System.Collections.Generic;
using System.Linq;
using Events;

namespace InMemoryEventStore
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly IEventPublisher _publisher;
        private readonly Dictionary<Guid, List<EventDescriptor>> _storedEvents;

        public InMemoryEventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
            _storedEvents = new Dictionary<Guid, List<EventDescriptor>>();
        }

        #region Implementation of IEventStore

        public IEnumerable<Event> GetEventsForAggregate(Guid aggregateId)
        {
            List<EventDescriptor> eventDescriptors;
            if (!_storedEvents.TryGetValue(aggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }
            return eventDescriptors.OrderBy(k => k.AggregateVersion).Select(d => d.EventData);
        }

        public IEnumerable<Event> GetEventsForAggregateSinceVersion(Guid aggregateId, int version)
        {
            return GetEventsForAggregate(aggregateId).Where(e => e.AggregateVersion > version).OrderBy(
                    e => e.AggregateVersion);
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
        {
            List<EventDescriptor> eventDescriptors;
            if (!_storedEvents.TryGetValue(aggregateId, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                _storedEvents.Add(aggregateId, eventDescriptors);
            } else if (eventDescriptors[eventDescriptors.Count - 1].AggregateVersion != expectedVersion && expectedVersion != 0)
            {
                throw new EventStoreConcurrencyException();
            }

            var i = expectedVersion;
            foreach (var @event in events)
            {
                i++;
                @event.AggregateVersion = i;
                eventDescriptors.Add(new EventDescriptor(aggregateId, i, @event));
                _publisher.Publish(@event);
            }
        }

        #endregion

        #region EventDescriptor struct

        private struct EventDescriptor
        {
            public Event EventData { get; private set; }
            public Guid AggregateId { get; private set; }
            public int AggregateVersion { get; private set; }

            public EventDescriptor(Guid aggregateId, int aggregateVersion, Event eventData) : this()
            {
                EventData = eventData;
                AggregateId = aggregateId;
                AggregateVersion = aggregateVersion;
            }
        }

        #endregion
    }
}