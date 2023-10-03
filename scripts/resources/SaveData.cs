using Godot;
using System;
using System.Collections.Generic;

public class SaveData : Node
{
    const string SAVEFILENAME = "PlayerData";
    
    const string SAVEPATH = $"user://{SAVEFILENAME}.tres";
    
    //---------Modifiable at runtime----------------------

    public List<RawInventoryItem> formattedInventoryItems;
    
    //---------/Modifiable at runtime----------------------
        
    void Save()
    {
        var rawSaveData = new RawSaveData()
        {
            inventoryItems = formattedInventoryItems
        };

        Variant jsonVariant = Variant.From(rawSaveData);

        string json = Json.Stringify(jsonVariant, "\t");
    }
}