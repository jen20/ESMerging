using System;

namespace Commands
{
    public class CreateInventoryItem : Command
    {
        public Guid InventoryItemId { get; private set; }
        public string Name { get; private set; }

        public CreateInventoryItem(Guid inventoryItemId, string name)
        {
            InventoryItemId = inventoryItemId;
            Name = name;
        }

        public override Guid AggregateId
        {
            get { return InventoryItemId; }
        }
    }
}