using UnityEngine;

[System.Serializable]
public enum ItemType
{
    Food,
    Drink,
    Toy,
    Medicine,
    Cleaning
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea(2, 5)]
    public string description;
    public Sprite icon;
    public ItemType itemType;
    
    [Header("Stats")]
    public int hungerRestore = 0;
    public int thirstRestore = 0;
    public int cleanlinessRestore = 0;
    public int energyRestore = 0;
    
    [Header("Item Properties")]
    public bool isStackable = true;
    public int maxStackSize = 99;
    public int value = 1;
    
    public virtual void UseItem(PetStats petStats)
    {
        if (petStats != null)
        {
            petStats.ModifyStats(hungerRestore, thirstRestore, cleanlinessRestore, energyRestore);
            Debug.Log($"Used {itemName}. Stats modified.");
        }
    }
}
