using System;
using System.Collections.Generic;
using CommandHandlers;
using Commands;
using Events;
using InMemoryEventStore;
using NUnit.Framework;

namespace Test.CommandHandlers
{
    [TestFixture]
    public class MergingHandlerTests
    {
        [Test]
        public void Fubar()
        {
            var allowedMerges = new AllowableMergesDefinition();
            allowedMerges.AllowBothWays<InventoryItemRenamed, InventoryItemCheckedOutFromStock>();
            allowedMerges.AllowBothWays<InventoryItemRenamed, InventoryItemReceivedIntoStock>();
            allowedMerges.AllowBothWays<InventoryItemReceivedIntoStock, InventoryItemCheckedOutFromStock>();
            allowedMerges.AllowOneWay<InventoryItemDeactivated, InventoryItemRenamed>();
            allowedMerges.AllowOneWay<InventoryItemDeactivated, InventoryItemCheckedOutFromStock>();
            allowedMerges.AllowOneWay<InventoryItemDeactivated, InventoryItemReceivedIntoStock>();

            var stubPublisher = new StubEventPublisher();
            var eventStore = new EventStore(stubPublisher);

            var aggregateId = Guid.NewGuid();

            var chain1 = new MergingContextCommitHandler<CreateInventoryItem>(
                            new CreateInventoryItemHandler(), eventStore, allowedMerges);

            var command1 = new CreateInventoryItem(aggregateId, "A Name");
            
            chain1.Handle(command1, new CommandExecutionContext());
            
            var repository = new InMemoryEventStoreRepository(eventStore);

            var chain2 = new RetryOnConcurencyExceptionHandler<DeactivateInventoryItem>(
                new MergingContextCommitHandler<DeactivateInventoryItem>(
                    new DeactivateInventoryItemHandler(repository), eventStore, allowedMerges));

            var command2 = new DeactivateInventoryItem(aggregateId, 1);

            chain2.Handle(command2, new CommandExecutionContext());
        }

    }
}