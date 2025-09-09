using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Projects.Shop
{
    public class ShopUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private TextMeshProUGUI shopNameText;
        [SerializeField] private TextMeshProUGUI playerMoneyText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Transform shopItemsContainer;
        [SerializeField] private GameObject shopItemUIPrefab;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button restockButton;
    
        [Header("Dependencies")]
        [SerializeField] private Shop shop;
        [SerializeField] private PlayerCurrency playerCurrency;
    
        private List<ShopItemUI> shopItemUIList = new List<ShopItemUI>();
    
        private void Awake()
        {
            if (shop == null)
                shop = FindObjectOfType<Shop>();
            
            if (playerCurrency == null)
                playerCurrency = FindObjectOfType<PlayerCurrency>();
        }
    
        private void OnEnable()
        {
            Shop.OnShopItemsChanged += UpdateShopDisplay;
            Shop.OnPurchaseResult += ShowMessage;
            Wallet.OnMoneyChanged += UpdateMoneyDisplay;
        }
    
        private void OnDisable()
        {
            Shop.OnShopItemsChanged -= UpdateShopDisplay;
            Shop.OnPurchaseResult -= ShowMessage;
            Wallet.OnMoneyChanged -= UpdateMoneyDisplay;
        }
    
        private void Start()
        {
            InitializeUI();
        
            if (shopPanel != null)
                shopPanel.SetActive(false);
        }
    
        private void InitializeUI()
        {
            // Настройка кнопок
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseShop);
            }
        
            if (restockButton != null)
            {
                restockButton.onClick.AddListener(RestockAllItems);
            }
        
            // Обновляем отображение
            if (shop != null && shopNameText != null)
            {
                shopNameText.text = shop.ShopName;
            }
        
            UpdateMoneyDisplay(playerCurrency != null ? playerCurrency.CurrentMoney : 0);
            ClearMessage();
        }
    
        public void OpenShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                UpdateShopDisplay(shop.ShopItems);
            }
        }
    
        public void CloseShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
            }
        }
    
        private void UpdateShopDisplay(ShopItem[] shopItems)
        {
            // Очищаем старые элементы UI
            ClearShopItems();
        
            if (shopItems == null || shopItemsContainer == null || shopItemUIPrefab == null)
                return;
        
            // Создаем новые элементы UI для каждого товара
            for (int i = 0; i < shopItems.Length; i++)
            {
                GameObject itemUIObj = Instantiate(shopItemUIPrefab, shopItemsContainer);
                ShopItemUI itemUI = itemUIObj.GetComponent<ShopItemUI>();
            
                if (itemUI != null)
                {
                    itemUI.Initialize(shopItems[i], shop, i);
                    shopItemUIList.Add(itemUI);
                }
            }
        }
    
        private void ClearShopItems()
        {
            foreach (ShopItemUI itemUI in shopItemUIList)
            {
                if (itemUI != null && itemUI.gameObject != null)
                {
                    Destroy(itemUI.gameObject);
                }
            }
            shopItemUIList.Clear();
        }
    
        private void UpdateMoneyDisplay(int currentMoney)
        {
            if (playerMoneyText != null)
            {
                playerMoneyText.text = $"{currentMoney} {playerCurrency.CurrencyName}";
            }
        }
    
        private void ShowMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
                messageText.gameObject.SetActive(true);
            
                // Скрываем сообщение через 3 секунды
                Invoke(nameof(ClearMessage), 3f);
            }
        }
    
        private void ClearMessage()
        {
            if (messageText != null)
            {
                messageText.text = "";
                messageText.gameObject.SetActive(false);
            }
        }
    
        private void RestockAllItems()
        {
            if (shop != null)
            {
                shop.RestockAllItems();
                ShowMessage("Товары пополнены!");
            }
        }
    
        public void RefreshShopDisplay()
        {
            foreach (ShopItemUI itemUI in shopItemUIList)
            {
                if (itemUI != null)
                {
                    itemUI.RefreshDisplay();
                }
            }
        }
    }
}
