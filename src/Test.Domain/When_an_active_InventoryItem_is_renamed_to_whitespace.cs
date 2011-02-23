using System;
using System.Collections.Generic;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_an_active_InventoryItem_is_renamed_to_whitespace : AggregateRootTestFixture<InventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, "Product Name");
        }

        protected override void When()
        {
            SubjectUnderTest.ChangeName("    ");
        }

        [Test]
        public void An_ArgumentException_is_thrown()
        {
            Assert.That(CaughtException is ArgumentException);
        }

        [Test]
        public void No_events_are_produced()
        {
            Assert.That(ProducedEvents.CountIs(0));
        }
    }
}