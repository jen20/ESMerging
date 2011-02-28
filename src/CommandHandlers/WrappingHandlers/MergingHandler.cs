using Commands;
using Domain;
using Events;
using InMemoryEventStore;

namespace CommandHandlers
{
    public class MergingHandler<TCommandType, TAggregateType> : ICommandHandler<TCommandType>
        where TCommandType : Command
        where TAggregateType : AggregateRootBase
    {
        private readonly ICommandHandler<TCommandType> _next;
        private readonly ITrackingRepository<TAggregateType> _repository;
        private readonly IEventStore _eventStore;

        public MergingHandler(ICommandHandler<TCommandType> next, ITrackingRepository<TAggregateType> repository, IEventStore eventStore)
        {
            _next = next;
            _repository = repository;
            _eventStore = eventStore;
        }

        public void Handle(TCommandType command)
        {
            var eventsSinceExpectedVersion = _eventStore.GetEventsForAggregateSinceVersion(command.AggregateId, command.ExepectedAggregateVersion);
            _next.Handle(command);
            var eventsFromCommandHandler = _repository.TrackedAggregate.GetUncommittedChanges();

            foreach (var existingEvent in eventsSinceExpectedVersion)
            {
                foreach (var proposedEvent in eventsFromCommandHandler)
                {
                    if (ConflictsWith(proposedEvent, existingEvent))
                        throw new RealConcurrencyException();
                }
            }

            _repository.CommitTrackedAggregate();
        }

        private static bool ConflictsWith(Event proposedEvent, Event existingEvent)
        {
            return false;
        }
    }
}