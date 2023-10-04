using Godot;
using System;
using System.Linq;

public partial class ItemData : Node {

    [Export] public string ItemsDirectory = "res://assets/resources/items/";
    public static Godot.Collections.Array<Item> items = new Godot.Collections.Array<Item>();

    public override void _Ready() {
        _LoadItemsFromPath();
    }

    void _LoadItemsFromPath() {

        using var dir = DirAccess.Open(ItemsDirectory);
        // Open item directory
        if (dir != null) {
            dir.ListDirBegin();
            string fileName = dir.GetNext();

            // Add all items from directory to resource array
            while (fileName != "") {
                string filePath = ItemsDirectory + fileName;
                var resource = (Item)GD.Load(filePath);
                items.Add(resource as Item);

                fileName = dir.GetNext();
            }
        }
    }

    public static Item GetItemById(int id)
    {
        return items.First(item => item.ID == id);
    }
}
