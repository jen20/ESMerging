using System;
using System.Collections.Generic;
using Events;

namespace CommandHandlers
{
    public class AllowableMergesDefinition
    {
        private readonly ISet<KeyValuePair<Type, Type>> _allowedMerges;
        
        public AllowableMergesDefinition()
        {
             _allowedMerges = new HashSet<KeyValuePair<Type, Type>>();
        }

        public void AllowOneWay<TProposed, TExisting>() where TProposed : Event where TExisting : Event
        {
            var newPair = new KeyValuePair<Type, Type>(typeof (TProposed), typeof (TExisting));
            _allowedMerges.Add(newPair);
        }

        public void AllowConsecutiveEvents<TEvent>() where TEvent : Event
        {
            AllowOneWay<TEvent, TEvent>();
        }

        public void AllowBothWays<TEvent1, TEvent2>() where TEvent1 : Event where TEvent2 : Event
        {
            AllowOneWay<TEvent1, TEvent2>();
            AllowOneWay<TEvent2, TEvent1>();
        }

        public bool IsMergeAllowed(Event proposedEvent, Event existingEvent)
        {
            var lookupPair = new KeyValuePair<Type, Type>(proposedEvent.GetType(), existingEvent.GetType());

            if (_allowedMerges.Contains(lookupPair))
                return true;

            return false;
        }
    }
}