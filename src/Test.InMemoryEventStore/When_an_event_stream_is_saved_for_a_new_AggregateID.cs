using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Events;
using InMemoryEventStore;
using NUnit.Framework;

namespace Test.InMemoryEventStore
{
    [TestFixture]
    public class When_an_event_stream_is_saved_for_a_new_AggregateID
    {
        private StubEventPublisher _publisher;
        private IEventStore _eventStore;

        private readonly Guid _aggregateId = Guid.NewGuid();
        
        [SetUp]
        public void SetUp()
        {
            _publisher = new StubEventPublisher();
            _eventStore = new global::InMemoryEventStore.InMemoryEventStore(_publisher);

            _eventStore.SaveEvents(_aggregateId, EventStreamToSave(), -1);
        }

        [Test]
        public void The_events_should_be_published()
        {
            Assert.That(_publisher.PublishedEvents.Count() == EventStreamToSave().Count());
        }

        [Test]
        public void The_events_should_be_retrievable_using_GetEventsForAggregate()
        {
            var retrievedEvents = _eventStore.GetEventsForAggregate(_aggregateId);

            Assert.AreEqual(EventStreamToSave().Count(), retrievedEvents.Count());
        }

        [Test]
        public void The_types_of_the_events_should_be_the_same_as_those_saved()
        {
            //NOTE: This is a hack for the purposes of getting the thing done quickly
            var retrievedEvents = _eventStore.GetEventsForAggregate(_aggregateId).ToList();
            var expectedEvents = EventStreamToSave().ToList();

            for (var i = 0; i < expectedEvents.Count(); i++)
            {
                Assert.AreEqual(expectedEvents[i].GetType(), retrievedEvents[i].GetType());
            }
        }
        
        private IEnumerable<Event> EventStreamToSave()
        {
            yield return new InventoryItemCreated(_aggregateId, "Product Name");
            yield return new InventoryItemRenamed(_aggregateId, "New Product Name");
            yield return new InventoryItemReceivedIntoStock(_aggregateId, 50);
            yield return new InventoryItemCheckedOutFromStock(_aggregateId, 10);
            yield return new InventoryItemCheckedOutFromStock(_aggregateId, 11);
            yield return new InventoryItemCheckedOutFromStock(_aggregateId, 12);
            yield return new InventoryItemDeactivated(_aggregateId);
        }
    }
}
