using System;
using System.Collections.Generic;
using Events;

namespace Domain
{
    public abstract class AggregateRootBase
    {
        #region Identification and Versioning

        public abstract Guid AggregateId { get; }
        public int AggregateVersion { get; private set; }

        #endregion

        #region Uncommitted Change Tracking and Exposure

        private readonly List<Event> _changes = new List<Event>();

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void ClearChanges()
        {
            _changes.Clear();
        }

        #endregion

        #region Application of events in derived classes

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);

            if (isNew)
                _changes.Add(@event);
        }

        #endregion

        #region Loading from Event Stream

        public void LoadFromEventStream(IEnumerable<Event> eventStream)
        {
            foreach (var e in eventStream)
            {
                ApplyChange(e, false);
                AggregateVersion = e.AggregateVersion;
            }
        }

        #endregion
    }
}
