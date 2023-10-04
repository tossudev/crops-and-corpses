using Godot;
using System;

public partial class PlayerInventoryData : Node {

    private const int PlayerInventorySize = 30;

    public static Godot.Collections.Array PlayerInventory = new Godot.Collections.Array();
    

    public override void _Ready() { 
        // Test adding items to the inventory
        var emptyItem = new Godot.Collections.Dictionary<string, Variant>();
        emptyItem.Add("ID", -1);
        emptyItem.Add("Quantity", 0);

        var dummyItem = new Godot.Collections.Dictionary<string, Variant>();
        dummyItem.Add("ID", 0);
        dummyItem.Add("Quantity", 5);

        var dummyItem2 = new Godot.Collections.Dictionary<string, Variant>();
        dummyItem2.Add("ID", 1);
        dummyItem2.Add("Quantity", 5);


        for (int i = 0; i < 10; i++) {
            PlayerInventory.Add(dummyItem);
        }
        for (int i = 0; i < 5; i++) {
            PlayerInventory.Add(dummyItem2);
        }
        for (int i = 0; i < 15; i++) {
            PlayerInventory.Add(emptyItem);
        }

        // GD.Print(PlayerInventory);
    }

}
