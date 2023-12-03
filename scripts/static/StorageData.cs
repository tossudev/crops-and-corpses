using System.Linq;
using Godot;
using System.Threading.Tasks;
using Godot.Collections;


public static class StorageData
{
    public const int PLAYER_INVENTORY_MAX_SIZE = 40;
    
    public static async void AddDefaultResourcesToInventoryIfEmpty()
    {
        await TaskExtensions.SuspendWhile(() => !PlayerInventoryController.isInitialized, 100);

        await Task.Delay(1000);
        if (SaveData.organizedPlayerInventory.Any(item => item != null)) return;
        
        Item log = ItemData.GetItemById(0);
        await PlayerInventoryController.AddItemToInventory(
            new RawInventoryItem(log.ID, log.Name, 20, log.StackSize));
        
        
        Item iron = ItemData.GetItemById(1);
        await PlayerInventoryController.AddItemToInventory(
            new RawInventoryItem(iron.ID, iron.Name, 20, iron.StackSize));
        
        Item curePotion = ItemData.GetItemById(300);
        await PlayerInventoryController.AddItemToInventory(
            new RawInventoryItem(curePotion.ID, curePotion.Name, 15, curePotion.StackSize));
    }

    public static bool ExistsInStorage(Array<RawInventoryItem> rawArray, int itemId, int amountRequired)
    {
        int amountFound = rawArray.Where(rawItem => rawItem.id == itemId).Sum(rawItem => rawItem.quantity);

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
            
            
            indexToReturn = rawInventoryItem.indexInStorage;
            break;
        }
        
        if (indexToReturn == 0) 
            GD.PushWarning("Item with specified criteria didn't exist in inventory, returning 0");
        
        return indexToReturn;
    }
}
