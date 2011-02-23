using System;

namespace Events
{
    public class InventoryItemReceivedIntoStock : Event
    {
        public Guid InventoryItemId { get; private set; }
        public int Quantity { get; private set; }

        public InventoryItemReceivedIntoStock(Guid inventoryItemId, int quantity)
        {
            InventoryItemId = inventoryItemId;
            Quantity = quantity;
        }
    }
}