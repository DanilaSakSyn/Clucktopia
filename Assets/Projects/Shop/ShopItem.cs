using UnityEngine;

namespace Projects.Shop
{
    [System.Serializable]
    public class ShopItem
    {
        [Header("Shop Item Info")]
        public Item item;
        public int price;
        public int stock = -1; // -1 означает неограниченный запас
        public bool isAvailable = true;
    
        [Header("Display")]
        public Sprite shopIcon;
        [TextArea(2, 3)]
        public string shopDescription;
    
        public bool CanPurchase(int playerMoney, int quantity = 1)
        {
            if (!isAvailable || item == null) return false;
            if (playerMoney < price * quantity) return false;
            if (stock != -1 && stock < quantity) return false;
        
            return true;
        }
    
        public void Purchase(int quantity = 1)
        {
            if (stock != -1)
            {
                stock -= quantity;
                if (stock <= 0)
                {
                    stock = 0;
                    isAvailable = false;
                }
            }
        }
    
        public void RestockItem(int amount)
        {
            if (stock != -1)
            {
                stock += amount;
                isAvailable = true;
            }
        }
    }
}
