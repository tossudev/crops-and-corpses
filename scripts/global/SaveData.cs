using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Godot.Collections;
using Dictionary = Godot.Collections.Dictionary;

[GlobalClass]
public partial class SaveData : Node
{
    const string SAVEFOLDERNAME = "saves";
    const string SAVEFILENAME = "PlayerData.txt";
    static string directoryPath = "";
    static string fullPath = "";
    
    public const string TOWN_STATS_KEY = "townStats";
    public const string APPLIED_TOWN_STATS_KEY = "appliedTownStats";
    public const string INVENTORY_ITEMS_KEY = "inventoryItems";
    public const string ORGANIZED_INVENTORY_ITEMS_KEY = "organizedInventoryItems";
    
    //---------Modifiable at runtime----------------------
    
    public static RawTownStats townHallStats = new ();
    public static List<TownUpgrade> appliedUpgrades = new ();
    public static Array<RawInventoryItem> organizedPlayerInventory = new ();
    public static List<RawInventoryItem> totalInventoryItems = new ();

    public static bool savingInProgress = false;
    public static bool firstLoadComplete = false;
    public static bool inventorySyncInProgress = false;
    //---------/Modifiable at runtime----------------------

    public override void _Ready()
    {
        base._Ready();
        directoryPath = ProjectSettings.GlobalizePath($"user://{SAVEFOLDERNAME}");
        fullPath = directoryPath.PathJoin(SAVEFILENAME);

        LoadSaveDataIntoMemory();
    }
    
    static async Task Save()
    {
        savingInProgress = true;
        if (string.IsNullOrEmpty(directoryPath))
        {
            GD.PrintErr("Save path not set, can't save game!");
            savingInProgress = false;
            return;
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        var rawSaveData = new RawSaveData()
        {
            townStats = townHallStats,
            appliedUpgrades = appliedUpgrades,
            inventoryItems = totalInventoryItems,
            organizedInventoryItems = organizedPlayerInventory
        };
        
        Dictionary saveDictionary = rawSaveData.GetFullDataDictionary();
        
        string json = Json.Stringify(saveDictionary, "\t");

        
        try
        {
            await File.WriteAllTextAsync(fullPath, json);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        savingInProgress = false;
    }
    
    public static async Task<Dictionary> LoadData()
    {
        if (!File.Exists(fullPath)) return null;

        await TaskExtensions.SuspendWhile(() => savingInProgress);
        
        string data = "";
        
        try
        {
            data = await File.ReadAllTextAsync(fullPath);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        Json loadedJson = new();
        
        Error error = loadedJson.Parse(data);

        if (error == Error.Ok) return (Dictionary)loadedJson.Data;
        
        GD.PrintErr(error, ": SaveData Load");
        return null;
    }

    static async void LoadSaveDataIntoMemory()
    {
        Dictionary saveData = await LoadData();
        
        // Town stats
        TownManager.ReadTownDataFromFile(saveData, false);
        
        // Inventory Data
        await RawInventoryItem.ReadInventoryDataFromFile(saveData);

        firstLoadComplete = true;
    }
    
    
    public static async Task SyncInventory(bool doSync = true)
    {
        await TaskExtensions.SuspendWhile(() => inventorySyncInProgress);
        inventorySyncInProgress = true;
        
        
        await UpdateTotalItems(doSync);
        inventorySyncInProgress = false;
    }

    public static async Task SyncTownStats()
    {
        await Save();
    }

    static async Task UpdateTotalItems(bool doSync = true)
    {
        totalInventoryItems.Clear();

        try
        {
            foreach (RawInventoryItem item in organizedPlayerInventory)
            {
                if (item == null) continue; //Empty inv slot
            
                PlayerInventoryData.AddItemToTotalItems(item.id, item.quantity);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }


        if (savingInProgress)
        {
            GD.Print("Saving was queued for " +
                     await TaskExtensions.SuspendWhile(
                         () => savingInProgress, GD.Randi() % 200 + 50)+  " ms");
        }
        
        if (doSync) await Save();
    }
}