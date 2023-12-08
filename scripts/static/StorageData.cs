using System.Linq;
using Godot;
using System.Threading.Tasks;
using Godot.Collections;


public static class StorageData
{
    public const int PLAYER_INVENTORY_SIZE = 32;
    public const int HOTBAR_SIZE = 8;
    public const int TOWN_STORAGE_SIZE = 64;
    
    public static async void AddDefaultResourcesToInventoryIfEmpty()
    {
        await TaskExtensions.SuspendWhile(() => !PlayerInventoryController.isInitialized, 100);

        await Task.Delay(1000);
        if (SaveData.organizedPlayerInventory.Any(item => item != null)) return;
        
        AddDefaultResourceToHotbar(150, 1);
        AddDefaultResourceToHotbar(350, 1);
        AddDefaultResourceToHotbar(360, 1);
    }

    static async void AddDefaultResourceToHotbar(int ID, int quantity)
    {
        Item item = ItemData.GetItemById(ID);
        await PlayerInventoryController.AddItemToHotbar(
            new RawInventoryItem(item.ID, item.Name, quantity, item.StackSize));
    }

    public static bool ExistsInStorage(Array<RawInventoryItem> rawArray, int itemId, int amountRequired)
    {
        int amountFound = rawArray.Where(rawItem => rawItem != null && rawItem.id == itemId).Sum(rawItem => rawItem.quantity);

        return amountFound >= amountRequired;
    }
    
    public static bool ExistsInInventoryOrHotbar(int itemId, int amountRequired)
    {
        var rawArray = SaveData.organizedPlayerInventory;
        
        int amountFound = rawArray.Where(rawItem => rawItem != null && rawItem.id == itemId).
            Sum(rawItem => rawItem.quantity);

        
        rawArray = SaveData.playerHotbarItems;
        
        amountFound += rawArray.Where(rawItem => rawItem != null && rawItem.id == itemId)
            .Sum(rawItem => rawItem.quantity);
        
        return amountFound >= amountRequired;
    }

    /// <param name="rawArray"></param>
    /// <param name="itemId"> ID of item to search for </param>
    /// <param name="mustNotBeFull"> (optional) Item stack must contain space for at least 1 more item </param>
    /// <returns> index of first item in organized player inventory that matches the conditions OR 0 </returns>
    public static int GetFirstStackIndexOfItem(Array<RawInventoryItem> rawArray, int itemId, bool mustNotBeFull = false)
    {
        int indexToReturn = 0;
        
        foreach (var rawInventoryItem in rawArray)
        {
            if (rawInventoryItem == null) continue;

            if (rawInventoryItem.id != itemId) continue;

            if (mustNotBeFull && rawInventoryItem.quantity >= rawInventoryItem.stackSize) continue;
            
            
            indexToReturn = rawInventoryItem.indexInStorageArray;
            break;
        }
        
        if (indexToReturn == 0) 
            GD.PushWarning("Item with specified criteria didn't exist in inventory, returning 0");
        
        return indexToReturn;
    }
}
