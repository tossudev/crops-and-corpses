using System.Collections.Generic;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class RawInventoryItem : GodotObject
{
    public string name;
    public int id;
    public int quantity;
}

[System.Serializable]
public partial class RawSaveData : GodotObject
{
    public const string ITEM_NAME_KEY = "name";
    public const string ITEM_QUANTITY_KEY = "quantity";
    
    public List<RawInventoryItem> inventoryItems = new ();
    
    public Dictionary GetFullDataDictionary()
    {
        Dictionary fullDictionary = new();

        Dictionary inventoryItemsDict = new();
        inventoryItems.ForEach(item =>
        {
            inventoryItemsDict.Add(item.id, new Dictionary()
            {
                { ITEM_NAME_KEY, item.name },
                { ITEM_QUANTITY_KEY, item.quantity },
            });
        });
        
        fullDictionary.Add(SaveData.INVENTORY_ITEMS_KEY,inventoryItemsDict);

        return fullDictionary;
    }
}