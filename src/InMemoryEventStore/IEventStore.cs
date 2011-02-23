using System;
using System.Collections.Generic;
using Events;

namespace InMemoryEventStore
{
    public interface IEventStore
    {
        IEnumerable<Event> GetEventsForAggregate(Guid aggregateId);
        IEnumerable<Event> GetEventsForAggregateSinceVersion(Guid aggregateId, int version);

        void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion);
    }
}
