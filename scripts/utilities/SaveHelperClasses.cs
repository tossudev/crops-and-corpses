using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class RawInventoryItem : GodotObject
{
    public int id;
    public string name;
    public int quantity;
    public int stackSize;

    public RawInventoryItem(int id, string name, int quantity, int stackSize)
    {
        this.id = id;
        this.name = name;
        this.quantity = quantity;
        this.stackSize = stackSize;
    }

    public int SpaceRemainingInStack => stackSize - quantity;
}

[System.Serializable]
public partial class RawSaveData : GodotObject
{
    public const string ITEM_ID_KEY = "id";
    public const string ITEM_NAME_KEY = "name";
    public const string ITEM_QUANTITY_KEY = "quantity";
    public const string ITEM_STACKSIZE_KEY = "stackSize";
    
    public List<RawInventoryItem> inventoryItems = new ();
    public Array<RawInventoryItem> organizedInventoryItems = new ();
    
    public Dictionary GetFullDataDictionary()
    {
        Dictionary fullDictionary = new();

        // All items in inventory
        Dictionary inventoryItemsDict = new();
        inventoryItems.ForEach(item =>
        {
            inventoryItemsDict.Add(item.id, new Dictionary()
            {
                { ITEM_NAME_KEY, item.name },
                { ITEM_QUANTITY_KEY, item.quantity },
                {ITEM_STACKSIZE_KEY, item.stackSize}
            });
        });
        fullDictionary.Add(SaveData.INVENTORY_ITEMS_KEY,inventoryItemsDict);

        
        // Organized inventory
        Array organizedInventoryItemsDictArray = new();

        foreach (var rawInventoryItem in organizedInventoryItems)
        {
            if (rawInventoryItem == null) continue;
            
            organizedInventoryItemsDictArray.Add(new Dictionary()
            {
                { ITEM_ID_KEY, rawInventoryItem.id },
                { ITEM_NAME_KEY, rawInventoryItem.name },
                { ITEM_QUANTITY_KEY, rawInventoryItem.quantity },
                {ITEM_STACKSIZE_KEY, rawInventoryItem.stackSize}
            });
        }
        
        fullDictionary.Add(SaveData.ORGANIZED_INVENTORY_ITEMS_KEY, organizedInventoryItemsDictArray);

        
        return fullDictionary;
    }
}