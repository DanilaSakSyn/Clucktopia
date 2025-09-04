using UnityEngine;

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
        if (gameManager == null || inputManager == null || petStats == null || timeManager == null || inventory == null || inventoryUI == null)
        {
            Debug.LogError("One or more components are not assigned in the inspector.");
            return;
        }

        gameManager.Initialize(inputManager, petStats);
        petStats.Initialize(timeManager);
        
        // Инициализация UI инвентаря с необходимыми компонентами
        if (inventoryUI != null)
        {
            // InventoryUI сам найдет компоненты через SerializeField, но можно добавить проверку
            Debug.Log("Inventory system initialized.");
        }
    }
}