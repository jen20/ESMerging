using System.Collections.Generic;
using Events;
using InMemoryEventStore;

namespace Test.CommandHandlers
{
    public class StubEventPublisher : IEventPublisher
    {
        private readonly IList<Event> _publishedEvents;

        public StubEventPublisher()
        {
            _publishedEvents = new List<Event>();
        }

        #region Implementation of IEventPublisher

        public void Publish<T>(T @event) where T : Event
        {
            _publishedEvents.Add(@event);
        }

        #endregion

        public IEnumerable<Event> PublishedEvents
        {
            get { return _publishedEvents; }
        }

        public bool ContainsEvent(Event @event)
        {
            return _publishedEvents.Contains(@event);
        }

        public void ClearPublishedEvents()
        {
            _publishedEvents.Clear();
        }
    }
}