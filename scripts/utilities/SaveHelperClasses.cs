using System.Collections.Generic;
using Godot;

[System.Serializable]
public partial class RawInventoryItem : GodotObject
{
    public string name;
    public int id;
    public int quantity;

    public Godot.Collections.Dictionary GetFullDictionary()
    {
        Godot.Collections.Dictionary fullDictionary = new ()
        {
            {nameof(name), name},
            {nameof(quantity), quantity}
        };

        return fullDictionary;
    }
}

[System.Serializable]
public partial class RawSaveData : GodotObject
{
    public List<RawInventoryItem> inventoryItems = new ();
    
    public Godot.Collections.Dictionary GetFullDictionary()
    {
        Godot.Collections.Dictionary fullDictionary = new();

        inventoryItems.ForEach(item =>
        {
            fullDictionary.Add(item.id, item.quantity);
        });

        return fullDictionary;
    }
}