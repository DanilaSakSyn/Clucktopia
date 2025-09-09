using UnityEngine;

namespace Projects.Shop
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Shop Access")]
        [SerializeField] private KeyCode openShopKey = KeyCode.S;
        [SerializeField] private bool canOpenShop = true;
    
        [Header("Dependencies")]
        private ShopUI shopUI;
        private Shop shop;
    
        private void Awake()
        {
            shopUI = FindObjectOfType<ShopUI>();
            shop = FindObjectOfType<Shop>();
        }
    
        private void Update()
        {
            if (canOpenShop && Input.GetKeyDown(openShopKey))
            {
                ToggleShop();
            }
        }
    
        public void OpenShop()
        {
            if (shopUI != null && canOpenShop)
            {
                shopUI.OpenShop();
            }
        }
    
        public void CloseShop()
        {
            if (shopUI != null)
            {
                shopUI.CloseShop();
            }
        }
    
        public void ToggleShop()
        {
            if (shopUI != null && shopUI.gameObject.activeInHierarchy)
            {
                bool isShopOpen = shopUI.transform.GetChild(0).gameObject.activeSelf; // Предполагаем, что shopPanel первый дочерний объект
            
                if (isShopOpen)
                    CloseShop();
                else
                    OpenShop();
            }
            else
            {
                OpenShop();
            }
        }
    
        public void SetShopAccess(bool canAccess)
        {
            canOpenShop = canAccess;
        }
    }
}
