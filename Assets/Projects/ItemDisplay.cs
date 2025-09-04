using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject tooltip;
    [SerializeField] private TextMeshProUGUI tooltipTitle;
    [SerializeField] private TextMeshProUGUI tooltipDescription;
    [SerializeField] private Button useButton;
    
    [Header("Visual Settings")]
    [SerializeField] private Color emptySlotColor = Color.gray;
    [SerializeField] private Color normalSlotColor = Color.white;
    
    private InventorySlot currentSlot;
    private int slotIndex;
    private InventoryUI inventoryUI;
    
    private void Awake()
    {
        if (useButton != null)
        {
            useButton.onClick.AddListener(UseItem);
        }
        
        if (tooltip != null)
        {
            tooltip.SetActive(false);
        }
    }
    
    public void Initialize(InventoryUI inventory, int index)
    {
        inventoryUI = inventory;
        slotIndex = index;
    }
    
    public void UpdateDisplay(InventorySlot slot)
    {
        currentSlot = slot;
        
        if (slot == null || slot.IsEmpty())
        {
            gameObject.SetActive(false);
            // Пустой слот
            itemIcon.sprite = null;
            itemIcon.color = emptySlotColor;
            quantityText.text = "";
            
            if (useButton != null)
                useButton.interactable = false;
        }
        else
        {
            gameObject.SetActive(false);
            
            // Заполненный слот
            itemIcon.sprite = slot.item.icon;
            itemIcon.color = normalSlotColor;
            
            if (slot.quantity > 1)
            {
                quantityText.text = slot.quantity.ToString();
            }
            else
            {
                quantityText.text = "";
            }
            
            if (useButton != null)
                useButton.interactable = true;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentSlot != null && !currentSlot.IsEmpty())
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // Левый клик - использовать предмет
                UseItem();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Правый клик - показать контекстное меню или дополнительную информацию
                ShowItemInfo();
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSlot != null && !currentSlot.IsEmpty() && tooltip != null)
        {
            ShowTooltip();
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false);
        }
    }
    
    private void UseItem()
    {
        if (inventoryUI != null && currentSlot != null && !currentSlot.IsEmpty())
        {
            inventoryUI.UseItem(slotIndex);
        }
    }
    
    private void ShowTooltip()
    {
        if (tooltip != null && currentSlot != null && currentSlot.item != null)
        {
            tooltip.SetActive(true);
            
            if (tooltipTitle != null)
                tooltipTitle.text = currentSlot.item.itemName;
            
            if (tooltipDescription != null)
            {
                string description = currentSlot.item.description;
                
                // Добавляем информацию о восстановлении статов
                if (currentSlot.item.hungerRestore > 0)
                    description += $"\n+{currentSlot.item.hungerRestore} Голод";
                if (currentSlot.item.thirstRestore > 0)
                    description += $"\n+{currentSlot.item.thirstRestore} Жажда";
                if (currentSlot.item.cleanlinessRestore > 0)
                    description += $"\n+{currentSlot.item.cleanlinessRestore} Чистота";
                if (currentSlot.item.energyRestore > 0)
                    description += $"\n+{currentSlot.item.energyRestore} Энергия";
                
                tooltipDescription.text = description;
            }
        }
    }
    
    private void ShowItemInfo()
    {
        if (currentSlot != null && currentSlot.item != null)
        {
            Debug.Log($"Предмет: {currentSlot.item.itemName}\nОписание: {currentSlot.item.description}\nКоличество: {currentSlot.quantity}");
        }
    }
}
