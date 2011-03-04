using System;

namespace Commands
{
    public class RenameInventoryItem : Command
    {
        public Guid InventoryItemId { get; private set; }
        public string NewName { get; private set; }

        public RenameInventoryItem(Guid inventoryItemId, string newName, int expectedAggregateVersion)
        {
            InventoryItemId = inventoryItemId;
            NewName = newName;
            ExepectedAggregateVersion = expectedAggregateVersion;
        }

        public override Guid AggregateId
        {
            get { return InventoryItemId; }
        }
    }
}