using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Shop
{
    public class ShopItemUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI stockText;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private GameObject soldOutOverlay;
    
        private ShopItem shopItem;
        private Shop shop;
        private int shopItemIndex;
    
        public void Initialize(ShopItem item, Shop shopReference, int index)
        {
            shopItem = item;
            shop = shopReference;
            shopItemIndex = index;
        
            UpdateDisplay();
        
            if (purchaseButton != null)
            {
                purchaseButton.onClick.RemoveAllListeners();
                purchaseButton.onClick.AddListener(() => PurchaseItem());
            }
        }
    
        private void UpdateDisplay()
        {
            if (shopItem == null || shopItem.item == null) return;
        
            // Иконка товара
            if (itemIcon != null)
            {
                Sprite iconToUse = shopItem.shopIcon != null ? shopItem.shopIcon : shopItem.item.icon;
                itemIcon.sprite = iconToUse;
            }
        
            // Название товара
            if (itemNameText != null)
            {
                itemNameText.text = shopItem.item.itemName;
            }
        
            // Описание товара
            if (itemDescriptionText != null)
            {
                string description = !string.IsNullOrEmpty(shopItem.shopDescription) 
                    ? shopItem.shopDescription 
                    : shopItem.item.GetDescription();
                itemDescriptionText.text = description;
            }
        
            // Цена
            if (priceText != null)
            {
                priceText.text = $"{shopItem.price} <sprite index=2>";
            }
        
            // Количество в наличии
            if (stockText != null)
            {
                if (shopItem.stock == -1)
                {
                    stockText.text = "В наличии: ∞";
                }
                else
                {
                    stockText.text = $"В наличии: {shopItem.stock}";
                }
            }
        
            // Доступность покупки
            bool canPurchase = shopItem.isAvailable && shopItem.stock != 0;
        
            if (purchaseButton != null)
            {
                purchaseButton.interactable = canPurchase;
            }
        
            if (soldOutOverlay != null)
            {
                soldOutOverlay.SetActive(!canPurchase);
            }
        }
    
        private void PurchaseItem()
        {
            if (shop != null)
            {
                shop.PurchaseItem(shopItemIndex, 1);
                UpdateDisplay(); // Обновляем отображение после покупки
            }
        }
    
        public void RefreshDisplay()
        {
            UpdateDisplay();
        }
    }
}
