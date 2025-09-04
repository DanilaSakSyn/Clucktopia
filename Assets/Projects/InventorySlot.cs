using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;

    public InventorySlot()
    {
        item = null;
        quantity = 0;
    }

    public InventorySlot(Item newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;
    }

    public bool IsEmpty()
    {
        return item == null || quantity <= 0;
    }

    public bool CanAddItem(Item itemToAdd)
    {
        if (IsEmpty()) return true;
        return item == itemToAdd && item.isStackable && quantity < item.maxStackSize;
    }

    public int AddItem(Item itemToAdd, int quantityToAdd)
    {
        if (IsEmpty())
        {
            item = itemToAdd;
            quantity = quantityToAdd;
            return 0;
        }

        if (item == itemToAdd && item.isStackable)
        {
            int spaceLeft = item.maxStackSize - quantity;
            int amountToAdd = Mathf.Min(quantityToAdd, spaceLeft);
            quantity += amountToAdd;
            return quantityToAdd - amountToAdd;
        }

        return quantityToAdd;
    }

    public void RemoveItem(int quantityToRemove)
    {
        quantity -= quantityToRemove;
        if (quantity <= 0)
        {
            item = null;
            quantity = 0;
        }
    }

    public void ClearSlot()
    {
        item = null;
        quantity = 0;
    }
}
