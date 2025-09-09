using Projects.Shop;
using UnityEngine;

namespace Projects
{
    public class Bootstrap : MonoBehaviour
    {
        private static Bootstrap _instance;

        public static Bootstrap Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject bootstrapObject = new GameObject("Bootstrap");
                    _instance = bootstrapObject.AddComponent<Bootstrap>();
                    DontDestroyOnLoad(bootstrapObject);
                }
                return _instance;
            }
        }

        [SerializeField] private GameManager gameManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private PetStats petStats;
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private Inventory inventory;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private PlayerCurrency playerCurrency;
        [SerializeField] private Shop.Shop shop;
        [SerializeField] private ShopUI shopUI;
        [SerializeField] private PetStatsUI petStatsUI;
        [SerializeField] private PetStatsNotification petStatsNotification;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            if (gameManager == null || inputManager == null || petStats == null || 
                timeManager == null || inventory == null || inventoryUI == null ||
                playerCurrency == null || shop == null || shopUI == null ||
                petStatsUI == null || petStatsNotification == null)
            {
                Debug.LogError("One or more components are not assigned in the inspector.");
                return;
            }

            // Инициализация основных компонентов
            gameManager.Initialize(inputManager, petStats);
            petStats.Initialize(timeManager);
            
            // Инициализация системы валюты
            var t = Wallet.Instance;
            
            // Инициализация магазина с зависимостями
            InitializeShop();
            
            // UI компоненты инициализируются автоматически
            Debug.Log("Pet Stats UI system initialized successfully");
        }
        
        private void InitializeShop()
        {
            // Магазин автоматически найдет зависимости через FindObjectOfType в Awake
            // но мы можем установить их явно для лучшего контроля
            Debug.Log("Shop system initialized successfully");
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && playerCurrency != null)
            {
                // playerCurrency.SaveMoney();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && playerCurrency != null)
            {
                // playerCurrency.SaveMoney();
            }
        }
    }
}