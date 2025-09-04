using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 20;
    [SerializeField] private InventorySlot[] slots;
    
    public static event Action<InventorySlot[]> OnInventoryChanged;
    
    private void Awake()
    {
        InitializeInventory();
    }
    
    private void InitializeInventory()
    {
        slots = new InventorySlot[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            slots[i] = new InventorySlot();
        }
    }
    
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null) return false;
        
        int remainingQuantity = quantity;
        
        // Сначала пытаемся добавить в существующие стаки
        if (item.isStackable)
        {
            for (int i = 0; i < slots.Length && remainingQuantity > 0; i++)
            {
                if (slots[i].CanAddItem(item))
                {
                    remainingQuantity = slots[i].AddItem(item, remainingQuantity);
                }
            }
        }
        
        // Затем создаем новые стаки в пустых слотах
        for (int i = 0; i < slots.Length && remainingQuantity > 0; i++)
        {
            if (slots[i].IsEmpty())
            {
                remainingQuantity = slots[i].AddItem(item, remainingQuantity);
            }
        }
        
        OnInventoryChanged?.Invoke(slots);
        return remainingQuantity == 0;
    }
    
    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (item == null) return false;
        
        int remainingToRemove = quantity;
        
        for (int i = 0; i < slots.Length && remainingToRemove > 0; i++)
        {
            if (slots[i].item == item)
            {
                int amountToRemove = Mathf.Min(remainingToRemove, slots[i].quantity);
                slots[i].RemoveItem(amountToRemove);
                remainingToRemove -= amountToRemove;
            }
        }
        
        OnInventoryChanged?.Invoke(slots);
        return remainingToRemove == 0;
    }
    
    public bool UseItem(int slotIndex, PetStats petStats)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return false;
        
        InventorySlot slot = slots[slotIndex];
        if (slot.IsEmpty()) return false;
        
        slot.item.UseItem(petStats);
        slot.RemoveItem(1);
        
        OnInventoryChanged?.Invoke(slots);
        return true;
    }
    
    public int GetItemCount(Item item)
    {
        int count = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                count += slots[i].quantity;
            }
        }
        return count;
    }
    
    public InventorySlot[] GetSlots()
    {
        return slots;
    }
    
    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < slots.Length)
            return slots[index];
        return null;
    }
    
    public bool HasSpace()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty()) return true;
        }
        return false;
    }
    
    public void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        OnInventoryChanged?.Invoke(slots);
    }
}
