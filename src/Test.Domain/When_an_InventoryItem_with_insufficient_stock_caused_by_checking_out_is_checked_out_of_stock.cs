using System;
using System.Collections.Generic;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_an_InventoryItem_with_insufficient_stock_caused_by_checking_out_is_checked_out_of_stock : AggregateRootTestFixture<InventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();
        private const int _quantityToCheckOut = 5;

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, "Product Name");
            yield return new InventoryItemReceivedIntoStock(_inventoryItemId, 8);
            yield return new InventoryItemCheckedOutFromStock(_inventoryItemId, 5);
        }

        protected override void When()
        {
            SubjectUnderTest.CheckOutFromStock(_quantityToCheckOut);
        }

        [Test]
        public void An_InsufficientStockException_is_thrown()
        {
            Assert.That(CaughtException is InsufficientStockException);
        }

        [Test]
        public void No_events_are_produced()
        {
            Assert.That(ProducedEvents.CountIs(0));
        }
    }
}