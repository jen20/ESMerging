using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_an_InventoryItem_is_receieved_into_stock : AggregateRootTestFixture<InventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();
        private const int _quantityReceived = 10;


        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, "Product Name");
        }

        protected override void When()
        {
            SubjectUnderTest.ReceiveIntoStock(_quantityReceived);
        }
        
        [Test]
        public void An_InventoryItemReceivedIntoStock_event_is_produced()
        {
            Assert.That(ProducedEvents.First().IsOfType<InventoryItemReceivedIntoStock>());

            var @event = (InventoryItemReceivedIntoStock) ProducedEvents.First();
            Assert.AreEqual(_inventoryItemId, @event.InventoryItemId);
            Assert.AreEqual(_quantityReceived, @event.Quantity);
        }

        [Test]
        public void No_other_events_are_produced()
        {
            Assert.That(ProducedEvents.CountIs(1));
        }
    }
}