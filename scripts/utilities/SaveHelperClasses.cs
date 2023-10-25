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
    public int indexInOrganizedInventory;

    public RawInventoryItem(int id, string name, int quantity, int stackSize, int indexInOrganizedInventory = -1)
    {
        this.id = id;
        this.name = name;
        this.quantity = quantity;
        this.stackSize = stackSize;
        this.indexInOrganizedInventory = indexInOrganizedInventory;
    }

    public int SpaceRemainingInStack => stackSize - quantity;

    public bool isValidIndex => (indexInOrganizedInventory >= 0 &&
                                 indexInOrganizedInventory < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE);
}

[System.Serializable]
public partial class RawSaveData : GodotObject
{
    public const string ITEM_ID_KEY = "id";
    public const string ITEM_NAME_KEY = "name";
    public const string ITEM_QUANTITY_KEY = "quantity";
    public const string ITEM_STACKSIZE_KEY = "stackSize";
    public const string ITEM_ORGANIZED_INDEX_KEY = "indexInOrganizedInventory";
    
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
                {ITEM_STACKSIZE_KEY, rawInventoryItem.stackSize},
                {ITEM_ORGANIZED_INDEX_KEY, rawInventoryItem.indexInOrganizedInventory}
            });
        }
        
        fullDictionary.Add(SaveData.ORGANIZED_INVENTORY_ITEMS_KEY, organizedInventoryItemsDictArray);

        
        return fullDictionary;
    }
}