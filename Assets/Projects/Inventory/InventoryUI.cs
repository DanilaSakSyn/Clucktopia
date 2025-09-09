using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Button closeButton;
    
    [Header("Components")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private PetStats petStats;
    
    private ItemDisplay[] itemDisplays;
    private bool isInitialized = false;
    
    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
    }
    
    private void OnEnable()
    {
        if (!isInitialized)
        {
            InitializeUI();
        }
        
        Inventory.OnInventoryChanged += UpdateUI;
        RefreshInventory();
    }
    
    private void OnDisable()
    {
        Inventory.OnInventoryChanged -= UpdateUI;
    }
    
    private void InitializeUI()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory не назначен в InventoryUI!");
            return;
        }
        
        InventorySlot[] slots = inventory.GetSlots();
        itemDisplays = new ItemDisplay[slots.Length];
        
        // Создаем UI элементы для каждого слота
        for (int i = 0; i < slots.Length; i++)
        {
            GameObject slotObject;
            
            if (slotPrefab != null && slotsParent != null)
            {
                slotObject = Instantiate(slotPrefab, slotsParent);
            }
            else
            {
                // Если префаб не назначен, создаем простой слот
                slotObject = CreateSimpleSlot(i);
            }
            
            ItemDisplay itemDisplay = slotObject.GetComponent<ItemDisplay>();
            if (itemDisplay == null)
            {
                itemDisplay = slotObject.AddComponent<ItemDisplay>();
            }
            
            itemDisplay.Initialize(this, i);
            itemDisplays[i] = itemDisplay;
        }
        
        isInitialized = true;
    }
    
    private GameObject CreateSimpleSlot(int index)
    {
        GameObject slotObject = new GameObject($"Slot_{index}");
        slotObject.transform.SetParent(slotsParent);
        
        // Добавляем Image компонент для фона слота
        Image slotBackground = slotObject.AddComponent<Image>();
        slotBackground.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Создаем дочерний объект для иконки предмета
        GameObject iconObject = new GameObject("Icon");
        iconObject.transform.SetParent(slotObject.transform);
        Image iconImage = iconObject.AddComponent<Image>();
        
        // Создаем дочерний объект для текста количества
        GameObject textObject = new GameObject("Quantity");
        textObject.transform.SetParent(slotObject.transform);
        
        return slotObject;
    }
    
    private void UpdateUI(InventorySlot[] slots)
    {
        RefreshInventory();
    }
    
    public void RefreshInventory()
    {
        if (!isInitialized || inventory == null) return;
        
        InventorySlot[] slots = inventory.GetSlots();
        
        for (int i = 0; i < itemDisplays.Length && i < slots.Length; i++)
        {
            if (itemDisplays[i] != null)
            {
                itemDisplays[i].UpdateDisplay(slots[i]);
            }
        }
    }
    
    public void UseItem(int slotIndex)
    {
        if (inventory != null && petStats != null)
        {
            bool success = inventory.UseItem(slotIndex, petStats);
            if (success)
            {
                Debug.Log($"Предмет из слота {slotIndex} использован!");
            }
        }
        else
        {
            Debug.LogError("Inventory или PetStats не назначены!");
        }
    }
    
    public void OpenInventory()
    {
        gameObject.SetActive(true);
        RefreshInventory();
    }
    
    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }
    
    public void ToggleInventory()
    {
        if (gameObject.activeInHierarchy)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }
    
    // Публичные методы для добавления предметов (например, для системы магазина или подбора предметов)
    public bool AddItem(Item item, int quantity = 1)
    {
        if (inventory != null)
        {
            return inventory.AddItem(item, quantity);
        }
        return false;
    }
    
    public int GetItemCount(Item item)
    {
        if (inventory != null)
        {
            return inventory.GetItemCount(item);
        }
        return 0;
    }
}
