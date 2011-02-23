using System;
using System.Collections.Generic;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_an_inactive_InventoryItem_is_deactivated : AggregateRootTestFixture<InventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, "Product Name");
            yield return new InventoryItemDeactivated(_inventoryItemId);
        }

        protected override void When()
        {
            SubjectUnderTest.Deactivate();
        }

        [Test]
        public void An_InvalidOperationException_is_thrown()
        {
            Assert.That(CaughtException is InvalidOperationException);
        }

        [Test]
        public void No_events_are_produced()
        {
            Assert.That(ProducedEvents.CountIs(0));
        }
    }
}