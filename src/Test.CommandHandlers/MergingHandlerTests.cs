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
            _allowedMerges.AllowConsecutiveEvents<InventoryItemCheckedOutFromStock>();
            _allowedMerges.AllowConsecutiveEvents<InventoryItemReceivedIntoStock>();
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
        public void A_call_not_requiring_a_merge_commits_successfully()
        {
            var aggregateId = Guid.NewGuid();

            //Set up existing aggregate state - will be aggregate version 3
            var existingAggregateEvents = new List<Event>();
            existingAggregateEvents.Add(new InventoryItemCreated(aggregateId, "A Name"));
            existingAggregateEvents.Add(new InventoryItemReceivedIntoStock(aggregateId, 100));
            existingAggregateEvents.Add(new InventoryItemRenamed(aggregateId, "Some other name"));

            _eventStore.SaveEvents(aggregateId, existingAggregateEvents, 0);

            //Command has an expected version which matches the current aggregate version
            var command = new RenameInventoryItem(aggregateId, "Another new name", 3);

            var specificCommandHandler = new RenameInventoryItemHandler(_repository);
            var mergingChain = new MergingContextCommitHandler<RenameInventoryItem>(specificCommandHandler, _eventStore,
                                                                                    _allowedMerges);

            mergingChain.Handle(command, new CommandExecutionContext());

            var newAggregate = _repository.GetById<InventoryItem>(aggregateId);
            Assert.AreEqual(4, newAggregate.AggregateVersion);
        }

        [Test]
        public void A_merge_which_is_allowed_succeeds()
        {
            var aggregateId = Guid.NewGuid();
            
            //Set up existing aggregate state - will be at aggregate version 3
            var existingAggregateEvents = new List<Event>();
            existingAggregateEvents.Add(new InventoryItemCreated(aggregateId, "A Name"));
            existingAggregateEvents.Add(new InventoryItemReceivedIntoStock(aggregateId, 100));
            existingAggregateEvents.Add(new InventoryItemRenamed(aggregateId, "Some other name"));
            
            _eventStore.SaveEvents(aggregateId, existingAggregateEvents, 0);

            //Command has an earlier expected version than the current aggregate version
            var command = new DeactivateInventoryItem(aggregateId, 1);

            var specificCommandHandler = new DeactivateInventoryItemHandler(_repository);
            var mergingChain = new MergingContextCommitHandler<DeactivateInventoryItem>(specificCommandHandler,
                                                                                        _eventStore, _allowedMerges);

            mergingChain.Handle(command, new CommandExecutionContext());

            var newAggregate = _repository.GetById<InventoryItem>(aggregateId);
            Assert.AreEqual(4, newAggregate.AggregateVersion);
        }

        [Test]
        public void A_merge_which_is_not_allowed_throws_a_RealConcurrencyException()
        {
            var aggregateId = Guid.NewGuid();

            //Set up existing aggregate state - will be at aggregate version 4
            var existingAggregateEvents = new List<Event>();
            existingAggregateEvents.Add(new InventoryItemCreated(aggregateId, "A Name"));
            existingAggregateEvents.Add(new InventoryItemReceivedIntoStock(aggregateId, 100));
            existingAggregateEvents.Add(new InventoryItemDeactivated(aggregateId));

            _eventStore.SaveEvents(aggregateId, existingAggregateEvents, 0);

            //Command has an earlier expected version and conflicts with changes between expected and actual version
            var command = new RenameInventoryItem(aggregateId, "A new name", 1);

            var specificCommandHandler = new RenameInventoryItemHandler(_repository);
            var mergingChain = new MergingContextCommitHandler<RenameInventoryItem>(specificCommandHandler, _eventStore,
                                                                                    _allowedMerges);

            Exception caughtException = new ThereWasNoExceptionButOneWasExpectedException();
            try
            {
                mergingChain.Handle(command, new CommandExecutionContext());
            } catch (Exception e)
            {
                caughtException = e;
            }

            Assert.That(caughtException is RealConcurrencyException);
        }
    }
}