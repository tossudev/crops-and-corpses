using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class RawInventoryItem : GodotObject
{
    // ITEM KEYS
    public const string ITEM_ID_KEY = "id";
    public const string ITEM_NAME_KEY = "name";
    public const string ITEM_QUANTITY_KEY = "quantity";
    public const string ITEM_STACKSIZE_KEY = "stackSize";
    public const string ITEM_ORGANIZED_INDEX_KEY = "indexInStorage";
    
    public int id;
    public string name;
    public int quantity;
    public int stackSize;
    public int indexInStorage;

    public RawInventoryItem(int id, string name, int quantity, int stackSize, int indexInStorage = -1)
    {
        this.id = id;
        this.name = name;
        this.quantity = quantity;
        this.stackSize = stackSize;
        this.indexInStorage = indexInStorage;
    }

    public int SpaceRemainingInStack => stackSize - quantity;

    public bool HasValidIndexInArray(Array<RawInventoryItem> array)
    {
        return indexInStorage >= 0 && indexInStorage > array.Count - 1;
    }
    
    
    /// <summary>
    /// Reads inventory data from save data
    /// </summary>
    /// <param name="saveData"></param>
    public static async Task ReadInventoryDataFromFile(Dictionary saveData)
    {
        SaveData.organizedPlayerInventory.Clear();
        
        if (SaveData.organizedPlayerInventory.Count < StorageData.PLAYER_INVENTORY_MAX_SIZE)
        {
            // Init inventory array with null values
            for (int i = 0; i < StorageData.PLAYER_INVENTORY_MAX_SIZE; i++)
            {
                SaveData.organizedPlayerInventory.Add(null);
            }
        }

        if (saveData != null)
        {
            Array organizedInventoryItemData = (Array) saveData[SaveData.ORGANIZED_INVENTORY_ITEMS_KEY];
            await Task.Run(() =>
            {
                foreach (var rawItemVariant in organizedInventoryItemData)
                {
                    Dictionary itemDataDict = (Dictionary)rawItemVariant; 
                
                    RawInventoryItem convertedRawItem = new RawInventoryItem(
                        (int) itemDataDict[ITEM_ID_KEY],
                        (string) itemDataDict[ITEM_NAME_KEY],
                        (int) itemDataDict[ITEM_QUANTITY_KEY],
                        (int) itemDataDict[ITEM_STACKSIZE_KEY],
                        (int) itemDataDict[ITEM_ORGANIZED_INDEX_KEY]);
                
                    SaveData.organizedPlayerInventory[convertedRawItem.indexInStorage] = convertedRawItem;
                }

            });
        }
    }


    public static Dictionary GetAllItemsDict(List<RawInventoryItem> inventoryItems)
    {
        Dictionary inventoryItemsDict = new();
        inventoryItems.ForEach(item =>
        {
            inventoryItemsDict.Add(item.id, new Dictionary()
            {
                { ITEM_NAME_KEY, item.name },
                { ITEM_QUANTITY_KEY, item.quantity },
                { ITEM_STACKSIZE_KEY, item.stackSize}
            });
        });

        return inventoryItemsDict;
    }
    
    public static Array GetOrganizedItemsArray(Array<RawInventoryItem> organizedInventoryItems)
    {
        Array organizedInventoryItemsDictArray = new();

        foreach (var rawInventoryItem in organizedInventoryItems)
        {
            if (rawInventoryItem == null) continue;
            
            organizedInventoryItemsDictArray.Add(new Dictionary()
            {
                { ITEM_ID_KEY, rawInventoryItem.id },
                { ITEM_NAME_KEY, rawInventoryItem.name },
                { ITEM_QUANTITY_KEY, rawInventoryItem.quantity },
                { ITEM_STACKSIZE_KEY, rawInventoryItem.stackSize},
                { ITEM_ORGANIZED_INDEX_KEY, rawInventoryItem.indexInStorage}
            });
        }

        return organizedInventoryItemsDictArray;
    }
}