using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using InMemoryEventStore;
using NUnit.Framework;

namespace Test.InMemoryEventStore
{
    [TestFixture]  
    public class When_calling_GetEventsForAggregateSinceVersion_3_with_an_existing_event_stream
    {
        private IEventStore _eventStore;

        private readonly Guid _aggregateId = Guid.NewGuid();
        
        private List<Event> _retrievedEvents;

        [SetUp]
        public void SetUp()
        {
            _eventStore = new global::InMemoryEventStore.InMemoryEventStore(new StubEventPublisher());
            _eventStore.SaveEvents(_aggregateId, EventStreamToSave(), -1);

            _retrievedEvents = _eventStore.GetEventsForAggregateSinceVersion(_aggregateId, 3).ToList();
        }

        [Test]
        public void Events_since_version_3_should_be_returned()
        {
            Assert.AreEqual(3, _retrievedEvents.Count());
        }

        [Test]
        public void The_events_should_be_the_last_ones_saved()
        {
            var expectedEvents = EventStreamToSave().Skip(4).Take(3).ToList();

            for (var i = 0; i < expectedEvents.Count(); i++)
            {
                Assert.AreEqual(expectedEvents[i].GetType(), _retrievedEvents[i].GetType());
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