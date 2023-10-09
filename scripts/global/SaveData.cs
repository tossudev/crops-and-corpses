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
    
    public const string INVENTORY_ITEMS_KEY = "inventoryItems";
    public const string ORGANIZED_INVENTORY_ITEMS_KEY = "organizedInventoryItems";
    
    //---------Modifiable at runtime----------------------
    
    public static Array<RawInventoryItem> organizedPlayerInventory = new ();
    public static List<RawInventoryItem> totalInventoryItems = new ();

    public static bool savingInProgress = false;
    //---------/Modifiable at runtime----------------------

    public override void _Ready()
    {
        base._Ready();
        directoryPath = ProjectSettings.GlobalizePath($"user://{SAVEFOLDERNAME}");
        fullPath = directoryPath.PathJoin(SAVEFILENAME);
    }
    
    static Task Save()
    {
        savingInProgress = true;
        if (string.IsNullOrEmpty(directoryPath))
        {
            GD.PrintErr("Save path not set, can't save game!");
            savingInProgress = false;
            return null;
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        var rawSaveData = new RawSaveData()
        {
            inventoryItems = totalInventoryItems,
            organizedInventoryItems = organizedPlayerInventory
        };
        
        Dictionary saveDictionary = rawSaveData.GetFullDataDictionary();
        
        string json = Json.Stringify(saveDictionary, "\t");

        
        try
        {
            File.WriteAllText(fullPath, json);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        savingInProgress = false;
        return null;
    }
    
    public static Dictionary LoadData()
    {
        if (!File.Exists(fullPath)) return null;

        string data = "";
        
        try
        {
            data = File.ReadAllText(fullPath);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        Json loadedJson = new();
        
        Error error = loadedJson.Parse(data);

        if (error != Error.Ok)
        {
            GD.PrintErr(error, ": SaveData Load");
            
            return null;
        }

        return (Dictionary) loadedJson.Data;
    }
    
    public static void SyncInventory()
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        UpdateTotalItemsAndSave();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    static async Task UpdateTotalItemsAndSave()
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


        if (!savingInProgress)
        {
            await Save();
        }
        else {
            GD.Print("Saving was queued");
            int savingQueueTime_Ms = 0;

            while (savingQueueTime_Ms < 5000)
            {
                int timeout_Ms = (GD.Randi() % 2 == 0) ? 100 : 250;
                
                savingQueueTime_Ms += timeout_Ms;
                
                await Task.Delay(timeout_Ms);

                if (savingInProgress) continue;
                
                await Save();
                break;
            }
            GD.Print($"Total Queue Time: {savingQueueTime_Ms}ms");
        }
    }
}