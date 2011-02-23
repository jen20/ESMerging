using System;

namespace Events
{
    public class InventoryItemRenamed : Event
    {
        public Guid InventoryItemId { get; private set; }
        public string NewName { get; set; }

        public InventoryItemRenamed(Guid inventoryItemId, string newName)
        {
            InventoryItemId = inventoryItemId;
            NewName = newName;
        }
    }
}