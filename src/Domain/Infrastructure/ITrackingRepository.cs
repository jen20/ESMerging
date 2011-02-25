using System;

namespace Domain
{
    public interface ITrackingRepository<TAggregateType> where TAggregateType : AggregateRootBase
    {
        TAggregateType GetById(Guid id);
        TAggregateType GetByIdAndTrack(Guid aggregateId);

        TAggregateType TrackedAggregate { get; }
        void TrackNewAggregate(TAggregateType aggregate);
        void CommitTrackedAggregate();
    }
}