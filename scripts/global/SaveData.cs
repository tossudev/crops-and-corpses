using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using FileAccess = Godot.FileAccess;

[GlobalClass]
public partial class SaveData : Node
{
    const string SAVEFOLDERPATH = "user://saves";
    const string SAVEFILENAME = "PlayerData.txt";
    
    //---------Modifiable at runtime----------------------

    public static List<RawInventoryItem> currentInventoryItems = new List<RawInventoryItem>();
    
    //---------/Modifiable at runtime----------------------

    public static void Save()
    {
        DirAccess.MakeDirAbsolute(SAVEFOLDERPATH);
        
        var rawSaveData = new RawSaveData()
        {
            inventoryItems = currentInventoryItems
        };

        
        Godot.Collections.Dictionary saveDictionary = rawSaveData.GetFullDictionary();
        
        string json = Json.Stringify(saveDictionary, "\t");

        
        using var saveFile = FileAccess.Open($"{SAVEFOLDERPATH}/{SAVEFILENAME}", FileAccess.ModeFlags.Write);
        
        saveFile.StoreString(json);
    }
}