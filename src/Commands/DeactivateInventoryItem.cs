using System;

namespace Commands
{
    public class DeactivateInventoryItem : Command
    {
        public Guid InventoryItemId { get; private set; }

        public DeactivateInventoryItem(Guid inventoryItemId)
        {
            InventoryItemId = inventoryItemId;
        }

        public override Guid AggregateId
        {
            get { return InventoryItemId; }
        }
    }
}