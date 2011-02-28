using Commands;
using InMemoryEventStore;

namespace CommandHandlers
{
    public class RetryOnConcurencyExceptionHandler<T> : ICommandHandler<T> where T : Command
    {
        private readonly ICommandHandler<T> _next;

        public RetryOnConcurencyExceptionHandler(ICommandHandler<T> next)
        {
            _next = next;
        }

        public void Handle(T command)
        {
            bool retry = true;
            while (retry)
            {
                try
                {
                    _next.Handle(command);
                    retry = false;
                } catch (EventStoreConcurrencyException)
                {
                    //Specifically ignore and retry
                }
            }
        }
    }
}
