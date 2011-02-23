using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_an_active_InventoryItem_is_deactivated : AggregateRootTestFixture<InventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, "Product Name");
        }

        protected override void When()
        {
            SubjectUnderTest.Deactivate();
        }

        [Test]
        public void An_InventoryItemDeactivated_event_is_produced()
        {
            Assert.That(ProducedEvents.First().IsOfType<InventoryItemDeactivated>());

            var @event = (InventoryItemDeactivated) ProducedEvents.First();
            Assert.AreEqual(_inventoryItemId, @event.InventoryItemId);
        }

        [Test]
        public void No_other_events_are_produced()
        {
            Assert.That(ProducedEvents.CountIs(1));
        }
    }
}