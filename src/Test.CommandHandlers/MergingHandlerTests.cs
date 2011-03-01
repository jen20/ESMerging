using System;
using System.Collections.Generic;
using CommandHandlers;
using Commands;
using Domain;
using Events;
using InMemoryEventStore;
using NUnit.Framework;

namespace Test.CommandHandlers
{
    [TestFixture]
    public class MergingHandlerTests
    {
        [Test]
        public void Fubar()
        {
            var stubPublisher = new StubEventPublisher();
            var eventStore = new EventStore(stubPublisher);
            var repository = new InMemoryEventStoreRepository(eventStore);

            var aggregateId = Guid.NewGuid();

            var chain1 = new CreateInventoryItemHandler(repository);
            var command1 = new CreateInventoryItem(aggregateId, "A Name");
            
            chain1.Handle(command1);
            

            repository = new InMemoryEventStoreRepository(eventStore);

            var chain2 = new RetryOnConcurencyExceptionHandler<DeactivateInventoryItem>(
                            new MergingHandler<DeactivateInventoryItem>(
                                new DeactivateInventoryItemHandler(repository), repository, eventStore));
            var command2 = new DeactivateInventoryItem(aggregateId, 1);

            chain2.Handle(command2);
        }

    }

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