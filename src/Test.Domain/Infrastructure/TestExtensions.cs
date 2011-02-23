using System.Collections.Generic;
using System.Linq;
using Events;

namespace Test.Domain
{
    public static class TestExtensions
    {
        public static bool CountIs(this IEnumerable<Event> events, int value)
        {
            return events.ToList().Count == value;
        }
        public static bool IsOfType<TType>(this Event @event)
        {
            return @event.GetType().Equals(typeof(TType));
        }
    }
}