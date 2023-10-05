using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

[GlobalClass]
public partial class SaveData : Node
{
    const string SAVEFOLDERNAME = "saves";
    const string SAVEFILENAME = "PlayerData.txt";
    static string directoryPath = "";
    static string fullPath = "";
    
    public const string INVENTORY_ITEMS_KEY = "inventoryItems";
    
    //---------Modifiable at runtime----------------------

    public static List<RawInventoryItem> currentInventoryItems = new List<RawInventoryItem>();
    
    //---------/Modifiable at runtime----------------------

    public override void _Ready()
    {
        base._Ready();
        directoryPath = ProjectSettings.GlobalizePath($"user://{SAVEFOLDERNAME}");
        fullPath = directoryPath.PathJoin(SAVEFILENAME);
        Load();
    }

    public static void Save()
    {
        if (string.IsNullOrEmpty(directoryPath))
        {
            GD.PrintErr("Save path not set, can't save game!");
            return;
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        var rawSaveData = new RawSaveData()
        {
            inventoryItems = currentInventoryItems
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
    }
    
    public static void Load()
    {
        if (!File.Exists(fullPath)) return;

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
            GD.PrintErr(error);
            
            return;
        }

        Dictionary loadedData = (Dictionary) loadedJson.Data;

        currentInventoryItems.Clear();

        var inventoryItemsRaw = (Dictionary)loadedData[INVENTORY_ITEMS_KEY];
        
        foreach (var rawitem in inventoryItemsRaw)
        {
            Dictionary itemValue = (Dictionary)rawitem.Value;
            
            currentInventoryItems.Add(new RawInventoryItem()
            {
                id = (int)rawitem.Key,
                name = (string) itemValue[RawSaveData.ITEM_NAME_KEY],
                quantity = (int) itemValue[RawSaveData.ITEM_QUANTITY_KEY]
            });
        }
    }
}