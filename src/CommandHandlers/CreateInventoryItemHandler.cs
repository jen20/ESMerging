using Commands;
using Domain;

namespace CommandHandlers
{
    public class CreateInventoryItemHandler : ICommandHandler<CreateInventoryItem>
    {
        private readonly ITrackingRepository<InventoryItem> _trackingRepository;

        public CreateInventoryItemHandler(ITrackingRepository<InventoryItem> trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        public void Handle(CreateInventoryItem command)
        {
            var item = new InventoryItem(command.InventoryItemId, command.Name);
            _trackingRepository.TrackNewAggregate(item);
        }
    }
}