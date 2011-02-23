using System;
using System.Collections.Generic;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public class When_a_negative_quantity_is_receieved_into_stock : AggregateRootTestFixture<InventoryItem>
    {
        protected override IEnumerable<Event> Given()
        {
            yield return new InventoryItemCreated(Guid.NewGuid(), "Product Name");
        }

        protected override void When()
        {
            SubjectUnderTest.ReceiveIntoStock(-1);
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