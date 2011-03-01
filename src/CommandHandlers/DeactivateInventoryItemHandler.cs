using Commands;
using Domain;

namespace CommandHandlers
{
    public class DeactivateInventoryItemHandler : ICommandHandler<DeactivateInventoryItem>
    {
        private readonly IRepository _repository;

        public DeactivateInventoryItemHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DeactivateInventoryItem command, CommandExecutionContext context)
        {
            var item = _repository.GetById<InventoryItem>(command.InventoryItemId);
            context.Aggregate = item;

            item.Deactivate();
        }
    }
}