using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_an_InventoryItem_with_sufficient_stock_is_checked_out_of_stock : AggregateRootTestFixture<InventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();
        private const int _quantityToCheckOut = 5;

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, "Product Name");
            yield return new InventoryItemReceivedIntoStock(_inventoryItemId, 10);
        }

        protected override void When()
        {
            SubjectUnderTest.CheckOutFromStock(_quantityToCheckOut);
        }

        [Test]
        public void An_InventoryItemCheckedOutFromStock_event_is_produced()
        {
            Assert.That(TestExtensions.IsOfType<InventoryItemCheckedOutFromStock>(ProducedEvents.First()));

            var @event = (InventoryItemCheckedOutFromStock) ProducedEvents.First();
            Assert.AreEqual(_inventoryItemId, @event.InventoryItemId);
            Assert.AreEqual(_quantityToCheckOut, @event.Quantity);
        }

        [Test]
        public void No_other_events_are_produced()
        {
            Assert.That(ProducedEvents.CountIs(1));
        }
    }
}