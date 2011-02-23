using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_an_active_InventoryItem_is_renamed : AggregateRootTestFixture<InventoryItem>
    {
        private readonly Guid _inventoryItemId = Guid.NewGuid();
        private const string _newName = "Renamed Product";

        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(_inventoryItemId, "Product Name");
        }

        protected override void When()
        {
            SubjectUnderTest.ChangeName(_newName);
        }

        [Test]
        public void An_InventoryItemRenamed_event_is_produced()
        {
            Assert.That(TestExtensions.IsOfType<InventoryItemRenamed>(ProducedEvents.First()));

            var @event = (InventoryItemRenamed)ProducedEvents.First();
            Assert.AreEqual(_inventoryItemId, @event.InventoryItemId);
            Assert.AreEqual(_newName, @event.NewName);
        }

        [Test]
        public void No_other_events_are_produced()
        {
            Assert.That(ProducedEvents.CountIs(1));
        }
    }
}