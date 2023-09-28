using Godot;
using System;

public partial class ItemData : Node {

    [Export] public string ItemsDirectory = "res://assets/resources/items/";
    public Godot.Collections.Array<Item> Items = new Godot.Collections.Array<Item>();

    public override void _Ready() {
        _LoadItemsFromPath();
        GD.Print(Items);
        // _SortItemsUsingID();
        // GD.Print(Items);
    }

    private void _LoadItemsFromPath() {

        using var dir = DirAccess.Open(ItemsDirectory);

        // Open item directory
        if (dir != null) {
            dir.ListDirBegin();
            string fileName = dir.GetNext();

            // Add all items from directory to resource array
            while (fileName != "") {
                string filePath = ItemsDirectory + fileName;
                var resource = (Item)GD.Load(filePath);
                Items.Add(resource as Item);

                fileName = dir.GetNext();
            }
        }
    }


    private void _SortItemsUsingID() {
    //     Item item = Items[0];
        // Items = Items.Sort();

        // IEnumerable<Item> query = Items.OrderBy(ID => item.ID);

        // foreach (ItemData item in query) {
        //     GD.Print("{0}", item.ID);
        // }
    
    }


}
