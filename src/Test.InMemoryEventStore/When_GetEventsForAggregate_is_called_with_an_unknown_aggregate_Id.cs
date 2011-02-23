using System;
using InMemoryEventStore;
using NUnit.Framework;

namespace Test.InMemoryEventStore
{
    [TestFixture]
    public class When_GetEventsForAggregate_is_called_with_an_unknown_aggregate_Id
    {
        private IEventPublisher _publisher;
        private IEventStore _eventStore;

        [SetUp]
        public void SetUp()
        {
            _publisher = new StubEventPublisher();
            _eventStore = new EventStore(_publisher);
        }

        [Test]
        public void An_AggregateNotFound_Exception_is_thrown()
        {
            Exception caughtException = new ThereWasNoExceptionButOneWasExpectedException();

            try
            {
                _eventStore.GetEventsForAggregate(Guid.NewGuid());
            } catch (Exception e)
            {
                caughtException = e;
            }

            Assert.That(caughtException is AggregateNotFoundException);
        }
    }
}