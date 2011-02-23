using System;

namespace Events
{
    public class InventoryItemCreated : Event
    {
        public Guid InventoryItemId { get; private set; }
        public string Name { get; private set; }

        public InventoryItemCreated(Guid inventoryItemId, string name)
        {
            InventoryItemId = inventoryItemId;
            Name = name;
        }
    }
}