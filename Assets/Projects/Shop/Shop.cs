using System;
using System.Collections.Generic;
using UnityEngine;

namespace Projects.Shop
{
    public class Shop : MonoBehaviour
    {
        [Header("Shop Settings")]
        [SerializeField] private string shopName = "Pet Shop";
        [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
    
        [Header("Dependencies")]
        [SerializeField] private Inventory inventory;
        [SerializeField] private PlayerCurrency playerCurrency;
    
        public static event Action<ShopItem[]> OnShopItemsChanged;
        public static event Action<string> OnPurchaseResult;
    
        public ShopItem[] ShopItems => shopItems.ToArray();
        public string ShopName => shopName;
    
        private void Awake()
        {
            if (inventory == null)
                inventory = FindObjectOfType<Inventory>();
        
            if (playerCurrency == null)
                playerCurrency = FindObjectOfType<PlayerCurrency>();
        }
    
        private void Start()
        {
            OnShopItemsChanged?.Invoke(ShopItems);
        }
    
        public bool PurchaseItem(int shopItemIndex, int quantity = 1)
        {
            if (shopItemIndex < 0 || shopItemIndex >= shopItems.Count)
            {
                OnPurchaseResult?.Invoke("Неверный товар!");
                return false;
            }
        
            ShopItem shopItem = shopItems[shopItemIndex];
        
            if (shopItem == null || shopItem.item == null)
            {
                OnPurchaseResult?.Invoke("Товар недоступен!");
                return false;
            }
        
            // Проверяем возможность покупки
            if (!shopItem.CanPurchase(playerCurrency.CurrentMoney, quantity))
            {
                if (!shopItem.isAvailable)
                {
                    OnPurchaseResult?.Invoke($"{shopItem.item.itemName} закончился!");
                }
                else if (shopItem.stock != -1 && shopItem.stock < quantity)
                {
                    OnPurchaseResult?.Invoke($"В наличии только {shopItem.stock} шт. {shopItem.item.itemName}!");
                }
                else
                {
                    OnPurchaseResult?.Invoke($"Недостаточно денег! Нужно: {shopItem.price * quantity}");
                }
                return false;
            }
        
            // Проверяем, поместится ли товар в инвентарь
            if (!CanAddToInventory(shopItem.item, quantity))
            {
                OnPurchaseResult?.Invoke("Недостаточно места в инвентаре!");
                return false;
            }
        
            // Совершаем покупку
            int totalCost = shopItem.price * quantity;
        
            if (playerCurrency.SpendMoney(totalCost))
            {
                // Добавляем товар в инвентарь
                if (inventory.AddItem(shopItem.item, quantity))
                {
                    // Уменьшаем запас товара в магазине
                 //   shopItem.Purchase(quantity);
                
                    OnPurchaseResult?.Invoke($"Куплено: {quantity} x {shopItem.item.itemName} за {totalCost} монет!");
                    OnShopItemsChanged?.Invoke(ShopItems);
                    return true;
                }
                else
                {
                    // Возвращаем деньги, если не удалось добавить в инвентарь
                    playerCurrency.AddMoney(totalCost);
                    OnPurchaseResult?.Invoke("Не удалось добавить товар в инвентарь!");
                    return false;
                }
            }
        
            return false;
        }
    
        private bool CanAddToInventory(Item item, int quantity)
        {
            if (inventory == null || item == null) return false;
        
            // Простая проверка - пытаемся добавить в инвентарь и смотрим результат
            // Это не идеальное решение, но работает с текущей структурой инвентаря
            return true; // Предполагаем, что AddItem в Inventory уже проверяет место
        }
    
        public ShopItem GetShopItem(int index)
        {
            if (index >= 0 && index < shopItems.Count)
                return shopItems[index];
            return null;
        }
    
        public void AddShopItem(ShopItem newItem)
        {
            if (newItem != null && newItem.item != null)
            {
                shopItems.Add(newItem);
                OnShopItemsChanged?.Invoke(ShopItems);
            }
        }
    
        public void RemoveShopItem(int index)
        {
            if (index >= 0 && index < shopItems.Count)
            {
                shopItems.RemoveAt(index);
                OnShopItemsChanged?.Invoke(ShopItems);
            }
        }
    
        public void RestockItem(int index, int amount)
        {
            if (index >= 0 && index < shopItems.Count)
            {
                shopItems[index].RestockItem(amount);
                OnShopItemsChanged?.Invoke(ShopItems);
            }
        }
    
        public void RestockAllItems()
        {
            foreach (ShopItem shopItem in shopItems)
            {
                if (shopItem.stock != -1)
                {
                    shopItem.RestockItem(10); // Пополняем на 10 единиц
                }
            }
            OnShopItemsChanged?.Invoke(ShopItems);
        }
    }
}
