using System;

namespace Events
{
    public class InventoryItemDeactivated : Event
    {
        public Guid InventoryItemId { get; private set; }

        public InventoryItemDeactivated(Guid inventoryItemId)
        {
            InventoryItemId = inventoryItemId;
        }
    }
}