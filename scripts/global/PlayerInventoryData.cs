using System;
using System.Linq;
using Godot;
using System.Threading;
using System.Threading.Tasks;
using Godot.Collections;
using Array = Godot.Collections.Array;

[GlobalClass]
public partial class PlayerInventoryData : Node
{
    public const int PLAYER_INVENTORY_MAX_SIZE = 40;

    public override void _Ready()
    {
        TestAsyncAdd();
    }
    
    async void TestAsyncAdd()
    {
        await TaskExtensions.SuspendWhile(() => !PlayerInventoryController.isInitialized, 100);

        await Task.Delay(1000);
        if (SaveData.totalInventoryItems.Count > 0) return;
        
        Item log = ItemData.GetItemById(0);
        await PlayerInventoryController.AddItem(
            new RawInventoryItem(log.ID, log.Name, 20, log.StackSize));
        
        
        Item iron = ItemData.GetItemById(1);
        await PlayerInventoryController.AddItem(
            new RawInventoryItem(iron.ID, iron.Name, 20, iron.StackSize));
        
        Item curePotion = ItemData.GetItemById(300);
        await PlayerInventoryController.AddItem(
            new RawInventoryItem(curePotion.ID, curePotion.Name, 15, curePotion.StackSize));
    }

    public static bool AddItemToTotalItems(int itemId, int amount)
    {
        Item itemToAdd = ItemData.GetItemById(itemId);
        
        if (itemToAdd == null) return false;
        
        if (SaveData.totalInventoryItems.Exists(rawItem => rawItem.id == itemId))
        {
            SaveData.totalInventoryItems.Find(rawItem => rawItem.id == itemId)
                .quantity += amount;
        }
        else
        {
            SaveData.totalInventoryItems.Add(
                new RawInventoryItem(itemId, itemToAdd.Name, amount, itemToAdd.StackSize));
        }
        return true;
    }

    public static bool ExistsInInventory(int itemId, int amount)
    {
        return SaveData.totalInventoryItems.Exists(item => item.id == itemId && item.quantity >= amount);
    }
    
    /// <param name="itemId"> ID of item to search for </param>
    /// <param name="mustNotBeFull"> (optional) Item stack must contain space for at least 1 more item </param>
    /// <returns> index of first item in organized player inventory that matches the conditions OR 0 </returns>
    public static int GetFirstStackIndexOfItem(int itemId, bool mustNotBeFull = false)
    {
        int indexToReturn = 0;
        
        foreach (var rawInventoryItem in SaveData.organizedPlayerInventory)
        {
            if (rawInventoryItem == null) continue;

            if (rawInventoryItem.id != itemId) continue;

            if (mustNotBeFull && rawInventoryItem.quantity >= rawInventoryItem.stackSize) continue;
            
            
            indexToReturn = rawInventoryItem.indexInOrganizedInventory;
            break;
        }
        
        if (indexToReturn == 0) 
            GD.PushWarning("Item with specified criteria didn't exist in inventory, returning 0");
        
        return indexToReturn;
    }
}
