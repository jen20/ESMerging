using System;

namespace Commands
{
    public abstract class Command
    {
        public abstract Guid AggregateId { get; }
        public int ExepectedAggregateVersion { get; protected set; }

        protected Command()
        {
            ExepectedAggregateVersion = 0;
        }
    }
}
