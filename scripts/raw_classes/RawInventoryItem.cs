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
    public int indexInStorageArray;

    // Not saved, only runtime
    public StorageSlotType hostSlotType;
    public Array<RawInventoryItem> hostArray;
    public GridContainer hostGrid;
    
    
    public RawInventoryItem(int id, string name, int quantity, int stackSize, int indexInStorageArray = -1)
    {
        this.id = id;
        this.name = name;
        this.quantity = quantity;
        this.stackSize = stackSize;
        this.indexInStorageArray = indexInStorageArray;
    }

    public int SpaceRemainingInStack => stackSize - quantity;

    public bool HasValidIndexInArray()
    {
        return indexInStorageArray >= 0 && indexInStorageArray < hostArray.Count;
    }
    
    
    /// <summary>
    /// Reads inventory data from save data
    /// </summary>
    /// <param name="saveData"></param>
    public static async Task ReadStorageDataFromFile(Dictionary saveData)
    {
        InitArrayWithNullValues(SaveData.organizedPlayerInventory, StorageData.PLAYER_INVENTORY_SIZE);
        InitArrayWithNullValues(SaveData.playerHotbarItems, StorageData.HOTBAR_SIZE);
        InitArrayWithNullValues(SaveData.townStorageItems, StorageData.TOWN_STORAGE_SIZE);

        if (saveData != null)
        {
            await MapSaveData(SaveData.organizedPlayerInventory, (Array) saveData[SaveData.ORGANIZED_INVENTORY_ITEMS_KEY]);
            await MapSaveData(SaveData.playerHotbarItems, (Array) saveData[SaveData.HOTBAR_ITEMS_KEY]);
            await MapSaveData(SaveData.townStorageItems, (Array) saveData[SaveData.TOWN_STORAGE_ITEMS_KEY]);
        }
    }

    static void InitArrayWithNullValues(Array<RawInventoryItem> rawArray, int itemCount)
    {
        rawArray.Clear();
        
        // Init inventory array with null values
        for (int i = 0; i < itemCount; i++)
        {
            rawArray.Add(null);
        }
    }
    
    static async Task MapSaveData(Array<RawInventoryItem> rawItems, Array dataArray)
    {
        await Task.Run(() =>
        {
            foreach (var rawItemVariant in dataArray)
            {
                Dictionary itemDataDict = (Dictionary)rawItemVariant; 
                
                RawInventoryItem convertedRawItem = new RawInventoryItem(
                    (int) itemDataDict[ITEM_ID_KEY],
                    (string) itemDataDict[ITEM_NAME_KEY],
                    (int) itemDataDict[ITEM_QUANTITY_KEY],
                    (int) itemDataDict[ITEM_STACKSIZE_KEY],
                    (int) itemDataDict[ITEM_ORGANIZED_INDEX_KEY]);
                
                rawItems[convertedRawItem.indexInStorageArray] = convertedRawItem;
            }

        });
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
                { ITEM_ORGANIZED_INDEX_KEY, rawInventoryItem.indexInStorageArray}
            });
        }

        return organizedInventoryItemsDictArray;
    }
}