using Domain;

namespace CommandHandlers
{
    public class CommandExecutionContext
    {
        public AggregateRootBase Aggregate { get; set; }
    }
}