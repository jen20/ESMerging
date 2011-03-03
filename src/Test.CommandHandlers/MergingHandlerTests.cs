using System;
using System.Collections.Generic;
using CommandHandlers;
using Commands;
using Domain;
using Events;
using InMemoryEventStore;
using NUnit.Framework;

namespace Test.CommandHandlers
{
    [TestFixture]
    public class MergingHandlerTests
    {
        private AllowableMergesDefinition _allowedMerges;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _allowedMerges = new AllowableMergesDefinition();
            _allowedMerges.AllowBothWays<InventoryItemRenamed, InventoryItemCheckedOutFromStock>();
            _allowedMerges.AllowBothWays<InventoryItemRenamed, InventoryItemReceivedIntoStock>();
            _allowedMerges.AllowBothWays<InventoryItemReceivedIntoStock, InventoryItemCheckedOutFromStock>();
            _allowedMerges.AllowOneWay<InventoryItemDeactivated, InventoryItemRenamed>();
            _allowedMerges.AllowOneWay<InventoryItemDeactivated, InventoryItemCheckedOutFromStock>();
            _allowedMerges.AllowOneWay<InventoryItemDeactivated, InventoryItemReceivedIntoStock>();
        }

        private StubEventPublisher _eventPublisher;
        private IEventStore _eventStore;
        private IRepository _repository;

        [SetUp]
        public void TestSetUp()
        {
            _eventPublisher = new StubEventPublisher();
            _eventStore = new InMemoryEventStore.InMemoryEventStore(_eventPublisher);
            _repository = new EventStoreRepository(_eventStore);
        }

        [Test]
        public void An_allowed_merge_succeeds()
        {
            var aggregateId = Guid.NewGuid();
            
            //Set up existing aggregate state
            var existingAggregateEvents = new List<Event>();
            existingAggregateEvents.Add(new InventoryItemCreated(aggregateId, "A Name"));
            existingAggregateEvents.Add(new InventoryItemReceivedIntoStock(aggregateId, 100));
            existingAggregateEvents.Add(new InventoryItemRenamed(aggregateId, "Some other name"));
            
            _eventStore.SaveEvents(aggregateId, existingAggregateEvents, 0);

            var command = new DeactivateInventoryItem(aggregateId, 1);

            var specificCommandHandler = new DeactivateInventoryItemHandler(_repository);
            var mergingChain = new MergingContextCommitHandler<DeactivateInventoryItem>(specificCommandHandler,
                                                                                        _eventStore, _allowedMerges);

            mergingChain.Handle(command, new CommandExecutionContext());

            var newAggregate = _repository.GetById<InventoryItem>(aggregateId);
            Assert.AreEqual(4, newAggregate.AggregateVersion);
        }

    }
}