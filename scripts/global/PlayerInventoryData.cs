using System;
using Godot;
using System.Threading;
using System.Threading.Tasks;
using Godot.Collections;
using Array = Godot.Collections.Array;

[GlobalClass]
public partial class PlayerInventoryData : Node
{
    public const int PLAYER_INVENTORY_MAX_SIZE = 32;

    public override void _Ready()
    {
        TestAsyncAdd(new CancellationTokenSource());
    }

    /// <summary>
    /// Reads inventory data from save data
    /// </summary>
    /// <param name="saveData"></param>
    /// <remarks> INVENTORY MUST BE INITIALIZED BEFORE CALLING </remarks>
    public static async Task ReadInventoryDataFromFile(Dictionary saveData)
    {
        if (SaveData.organizedPlayerInventory.Count == 0)
        {
            GD.PrintErr("Inventory was not initialized with null values");
            return;
        }

        if (saveData == null) return;
        
        Array organizedInventoryItemData = (Array) saveData[SaveData.ORGANIZED_INVENTORY_ITEMS_KEY];
        await Task.Run(() =>
        {
            foreach (var rawItemVariant in organizedInventoryItemData)
            {
                Dictionary itemDataDict = (Dictionary)rawItemVariant; 
                
                RawInventoryItem convertedRawItem = new RawInventoryItem(
                    (int) itemDataDict[RawSaveData.ITEM_ID_KEY],
                    (string) itemDataDict[RawSaveData.ITEM_NAME_KEY],
                    (int) itemDataDict[RawSaveData.ITEM_QUANTITY_KEY],
                    (int) itemDataDict[RawSaveData.ITEM_STACKSIZE_KEY],
                    (int) itemDataDict[RawSaveData.ITEM_ORGANIZED_INDEX_KEY]);
                
                SaveData.organizedPlayerInventory[convertedRawItem.indexInOrganizedInventory] = convertedRawItem;
            }

            SaveData.SyncInventory();
        });
    }
    
    async Task TestAsyncAdd(CancellationTokenSource tokenSrc)
    {
        CancellationToken token = tokenSrc.Token;

        await Task.Delay(1000, token);
        
        if (SaveData.totalInventoryItems.Count > 0) return;
        
        Item log = ItemData.GetItemById(0);
        PlayerInventoryController.AddItem(
            new RawInventoryItem(log.ID, log.Name, 20, log.StackSize));
        
        
        Item iron = ItemData.GetItemById(1);
        PlayerInventoryController.AddItem(
            new RawInventoryItem(iron.ID, iron.Name, 20, iron.StackSize));
        
        Item curePotion = ItemData.GetItemById(10);
        PlayerInventoryController.AddItem(
            new RawInventoryItem(curePotion.ID, curePotion.Name, 15, curePotion.StackSize));
        
        tokenSrc.Dispose();
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
}
