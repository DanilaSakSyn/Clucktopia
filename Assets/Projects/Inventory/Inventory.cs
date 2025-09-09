using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class InventoryData
{
    public List<SlotData> slotData;
    
    public InventoryData()
    {
        slotData = new List<SlotData>();
    }
}

[System.Serializable]
public class SlotData
{
    public string itemName;
    public int quantity;
    
    public SlotData(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }
}

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 20;
    [SerializeField] private InventorySlot[] slots;
    
    private const string INVENTORY_SAVE_KEY = "PlayerInventory";
    
    public static event Action<InventorySlot[]> OnInventoryChanged;
    
    private void Awake()
    {
        InitializeInventory();
        LoadInventory(); // Автоматически загружаем инвентарь при запуске
    }
    
    private void InitializeInventory()
    {
        if (slots == null || slots.Length == 0)
        {
            slots = new InventorySlot[inventorySize];
            for (int i = 0; i < inventorySize; i++)
            {
                slots[i] = new InventorySlot();
            }
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
        
        SaveInventory(); // Автоматически сохраняем после изменения
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
        
        SaveInventory(); // Автоматически сохраняем после изменения
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
    
    /// <summary>
    /// Сохраняет инвентарь в PlayerPrefs
    /// </summary>
    public void SaveInventory()
    {
        InventoryData inventoryData = new InventoryData();
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty())
            {
                SlotData slotData = new SlotData(slots[i].item.name, slots[i].quantity);
                inventoryData.slotData.Add(slotData);
            }
            else
            {
                // Добавляем пустой слот для сохранения позиций
                SlotData emptySlot = new SlotData("", 0);
                inventoryData.slotData.Add(emptySlot);
            }
        }
        
        string jsonData = JsonUtility.ToJson(inventoryData);
        Debug.Log(jsonData);
        PlayerPrefs.SetString(INVENTORY_SAVE_KEY, jsonData);
        PlayerPrefs.Save();
        
        Debug.Log("Инвентарь сохранен");
    }
    
    /// <summary>
    /// Загружает инвентарь из PlayerPrefs
    /// </summary>
    public void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(INVENTORY_SAVE_KEY))
        {
            Debug.Log("Сохраненный инвентарь не найден");
            return;
        }
        
        string jsonData = PlayerPrefs.GetString(INVENTORY_SAVE_KEY);
        Debug.Log(jsonData);
        
        InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(jsonData);
        
        if (inventoryData == null || inventoryData.slotData == null)
        {
            Debug.LogWarning("Не удалось загрузить данные инвентаря");
            return;
        }
        
        // Очищаем текущий инвентарь
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        
        // Загружаем сохраненные данные
        for (int i = 0; i < inventoryData.slotData.Count && i < slots.Length; i++)
        {
            SlotData slotData = inventoryData.slotData[i];
            
            if (!string.IsNullOrEmpty(slotData.itemName) && slotData.quantity > 0)
            {
                Item item = LoadItemByName(slotData.itemName);
                if (item != null)
                {
                    slots[i].AddItem(item, slotData.quantity);
                }
            }
        }
        
        OnInventoryChanged?.Invoke(slots);
        Debug.Log("Инвентарь загружен");
    }
    
    /// <summary>
    /// Загружает предмет по имени из Resources
    /// </summary>
    private Item LoadItemByName(string itemName)
    {
        Item item = Resources.Load<Item>(itemName);
        if (item == null)
        {
            // Пытаемся найти в корне Assets
            item = Resources.Load<Item>("Items/" + itemName);
        }
        
        if (item == null)
        {
            Debug.LogWarning($"Не удалось найти предмет: {itemName}");
        }
        
        return item;
    }
    
    /// <summary>
    /// Очищает сохраненный инвентарь
    /// </summary>
    public void ClearSavedInventory()
    {
        PlayerPrefs.DeleteKey(INVENTORY_SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("Сохраненный инвентарь очищен");
    }
    
    /// <summary>
    /// Очищает весь инвентарь
    /// </summary>
    public void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        
        SaveInventory();
        OnInventoryChanged?.Invoke(slots);
        Debug.Log("Инвентарь очищен");
    }
}
