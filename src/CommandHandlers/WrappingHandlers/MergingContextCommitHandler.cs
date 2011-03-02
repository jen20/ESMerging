using System.Collections.Generic;
using Commands;
using Events;
using InMemoryEventStore;

namespace CommandHandlers
{
    public class MergingContextCommitHandler<TCommandType> : ICommandHandler<TCommandType> where TCommandType : Command
    {
        private readonly ICommandHandler<TCommandType> _next;
        private readonly IEventStore _eventStore;

        public MergingContextCommitHandler(ICommandHandler<TCommandType> next, IEventStore eventStore)
        {
            _next = next;
            _eventStore = eventStore;
        }

        public void Handle(TCommandType command, CommandExecutionContext context)
        {
            _next.Handle(command, context);

            if (context.Aggregate == null)
                return;

            IEnumerable<Event> eventsSinceExpectedVersion;

            try
            {
                eventsSinceExpectedVersion = _eventStore.GetEventsForAggregateSinceVersion(command.AggregateId, command.ExepectedAggregateVersion);
            } catch (AggregateNotFoundException)
            {
                eventsSinceExpectedVersion = new List<Event>();
            }

            var eventsFromCommandHandler = context.Aggregate.GetUncommittedChanges();

            foreach (var existingEvent in eventsSinceExpectedVersion)
            {
                foreach (var proposedEvent in eventsFromCommandHandler)
                {
                    if (ConflictsWith(proposedEvent, existingEvent))
                        throw new RealConcurrencyException();
                }
            }

            _eventStore.SaveEvents(context.Aggregate.AggregateId, context.Aggregate.GetUncommittedChanges(),
                                   command.ExepectedAggregateVersion);
        }

        private static bool ConflictsWith(Event proposedEvent, Event existingEvent)
        {
            return false;
        }
    }
}