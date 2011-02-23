using System;
using Events;

namespace Domain
{
    public class InventoryItem : AggregateRootBase
    {
        #region State and State Transitions

        private Guid _inventoryItemId;
        private bool _isActivated;
        private int _quantityInStock;

        private void Apply(InventoryItemCreated e)
        {
            _inventoryItemId = e.InventoryItemId;
            _isActivated = true;
            _quantityInStock = 0;
        }

        private void Apply(InventoryItemDeactivated e)
        {
            _isActivated = false;
        }

        private void Apply(InventoryItemReceivedIntoStock e)
        {
            _quantityInStock += e.Quantity;
        }

        private void Apply(InventoryItemCheckedOutFromStock e)
        {
            _quantityInStock -= e.Quantity;
        }

        #endregion

        #region Public API

        public InventoryItem(Guid id, string name)
        {
            ApplyChange(new InventoryItemCreated(id, name));
        }

        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Must have a non-whitespace name");

            ApplyChange(new InventoryItemRenamed(_inventoryItemId, newName));
        }

        public void Deactivate()
        {
            if (!_isActivated)
                throw new InvalidOperationException("Cannot dactivate an Inventory Item which is not active");

            ApplyChange(new InventoryItemDeactivated(_inventoryItemId));
        }

        public void ReceiveIntoStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Cannot receive 0 or negative quantity into stock");

            ApplyChange(new InventoryItemReceivedIntoStock(_inventoryItemId, quantity));
        }

        public void CheckOutFromStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Cannot check 0 or negative quantity out of stock");

            if (quantity > _quantityInStock)
                throw new InsufficientStockException();

            ApplyChange(new InventoryItemCheckedOutFromStock(_inventoryItemId, quantity));
        }

        #endregion

        #region Overrides of AggregateRootBase

        public override Guid AggregateId
        {
            get { return _inventoryItemId; }
        }

        #endregion

        #region Private constructor

        // ReSharper disable UnusedMember.Local
        private InventoryItem()
        {
        }
        // ReSharper restore UnusedMember.Local

        #endregion
    }
}