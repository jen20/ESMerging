using Events;

namespace InMemoryEventStore
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : Event;
    }
}