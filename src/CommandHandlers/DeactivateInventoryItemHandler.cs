using Commands;
using Domain;

namespace CommandHandlers
{
    public class DeactivateInventoryItemHandler : ICommandHandler<DeactivateInventoryItem>
    {
        private readonly ITrackingRepository<InventoryItem> _trackingRepository;

        public DeactivateInventoryItemHandler(ITrackingRepository<InventoryItem> trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        public void Handle(DeactivateInventoryItem command)
        {
            var item = _trackingRepository.GetByIdAndTrack(command.InventoryItemId);
            item.Deactivate();
        }
    }
}