using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using InMemoryEventStore;
using NUnit.Framework;

namespace Test.InMemoryEventStore
{
    [TestFixture]
    public class When_saving_events_with_an_earlier_expected_version
    {
        private StubEventPublisher _publisher;
        private IEventStore _eventStore;
        private Exception _caughtException;

        private readonly Guid _aggregateId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _publisher = new StubEventPublisher();
            _eventStore = new global::InMemoryEventStore.InMemoryEventStore(_publisher);
            _eventStore.SaveEvents(_aggregateId, InitialEventsToSave(), -1);
            _publisher.ClearPublishedEvents();
            
            try
            {
                _eventStore.SaveEvents(_aggregateId, AdditionalEventsToSave(), 3);
            } catch (Exception e)
            {
                _caughtException = e;
            }
        }

        [Test]
        public void An_EventStoreConcurrencyException_is_thrown()
        {
            Assert.That(_caughtException is EventStoreConcurrencyException);
        }

        [Test]
        public void No_events_are_published()
        {
            Assert.That(_publisher.PublishedEvents.Count() == 0);
        }

        private IEnumerable<Event> AdditionalEventsToSave()
        {
            yield return new InventoryItemCheckedOutFromStock(_aggregateId, 10);
        }

        private IEnumerable<Event> InitialEventsToSave()
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
