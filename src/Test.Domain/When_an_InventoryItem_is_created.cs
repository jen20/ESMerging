using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_an_InventoryItem_is_created : AggregateRootTestFixture<InventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();
        private const string _inventoryItemName = "A Product";

        protected override IEnumerable<Event> Given()
        {
            return new List<Event>();
        }

        protected override void When()
        {
            SubjectUnderTest = new InventoryItem(_inventoryItemId, _inventoryItemName);
        }

        [Test]
        public void An_InventoryItemCreated_event_is_produced()
        {
            Assert.That(ProducedEvents.First().IsOfType<InventoryItemCreated>());

            var @event = (InventoryItemCreated) ProducedEvents.First();
            Assert.AreEqual(_inventoryItemId, @event.InventoryItemId);
            Assert.AreEqual(_inventoryItemName, @event.Name);
        }

        [Test]
        public void The_AggregateID_property_of_the_InventoryItem_contains_the_item_ID()
        {
            Assert.AreEqual(_inventoryItemId, SubjectUnderTest.AggregateId);
        }

        [Test]
        public void No_other_events_are_produced()
        {
            Assert.That(ProducedEvents.CountIs(1));
        }
    }
}