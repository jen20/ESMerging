using Commands;
using Domain;

namespace CommandHandlers
{
    public class CreateInventoryItemHandler : ICommandHandler<CreateInventoryItem>
    {
        public void Handle(CreateInventoryItem command, CommandExecutionContext context)
        {
            var item = new InventoryItem(command.InventoryItemId, command.Name);
            context.Aggregate = item;
        }
    }
}