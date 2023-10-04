using Godot;
using System;

public partial class ItemData : Node {

    [Export] public string itemsDirectory = "res://assets/resources/items/";
    public static Godot.Collections.Array<Item> items = new Godot.Collections.Array<Item>();

    public override void _Ready() {
        LoadItemsFromPath();
        // SortItemsUsingID();
        // GD.Print(Items);
    }

    private void LoadItemsFromPath() {

        using var dir = DirAccess.Open(itemsDirectory);

        // Open item directory
        if (dir != null) {
            dir.ListDirBegin();
            string _fileName = dir.GetNext();

            // Add all items from directory to resource array
            while (_fileName != "") {
                string _filePath = itemsDirectory + _fileName;
                var _resource = (Item)GD.Load(_filePath);
                items.Add(_resource as Item);

                _fileName = dir.GetNext();
            }
        }
    }


    private void SortItemsUsingID() {
    //     Item item = Items[0];
        // Items = Items.Sort();

        // IEnumerable<Item> query = Items.OrderBy(ID => item.ID);

        // foreach (ItemData item in query) {
        //     GD.Print("{0}", item.ID);
        // }
    
    }


}
