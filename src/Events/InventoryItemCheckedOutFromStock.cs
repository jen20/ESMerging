using System;

namespace Events
{
    public class InventoryItemCheckedOutFromStock : Event
    {
        public Guid InventoryItemId { get; private set; }
        public int Quantity { get; private set; }

        public InventoryItemCheckedOutFromStock(Guid inventoryItemId, int quantity)
        {
            InventoryItemId = inventoryItemId;
            Quantity = quantity;
        }
    }
}