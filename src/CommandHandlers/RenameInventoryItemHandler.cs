using Commands;
using Domain;

namespace CommandHandlers
{
    public class RenameInventoryItemHandler : ICommandHandler<RenameInventoryItem>
    {
        private readonly IRepository _repository;

        public RenameInventoryItemHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(RenameInventoryItem command, CommandExecutionContext context)
        {
            var item = _repository.GetById<InventoryItem>(command.InventoryItemId);
            context.Aggregate = item;

            item.ChangeName(command.NewName);
        }
    }
}