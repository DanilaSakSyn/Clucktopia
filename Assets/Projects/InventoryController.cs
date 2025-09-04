using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;
    
    [Header("References")]
    [SerializeField] private InventoryUI inventoryUI;
    
    private void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            if (inventoryUI != null)
            {
                inventoryUI.ToggleInventory();
            }
        }
    }
    
    // Публичные методы для добавления предметов (например, для тестирования или других систем)
    public void AddTestItems()
    {
        if (inventoryUI != null)
        {
            // Эти методы можно использовать для тестирования после создания ScriptableObject предметов
            Debug.Log("Добавьте предметы через ScriptableObject и вызовите inventoryUI.AddItem()");
        }
    }
}
