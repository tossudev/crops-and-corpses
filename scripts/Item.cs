using System.Diagnostics;
using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class Item : Resource {

    [Export] public int ID { get; set; }
    [Export] public string Name { get; set; }
    [Export] public Texture2D IconTexture { get; set; }
    [Export] public int StackSize { get; set; }

    public void _Ready() {
        ID = 0;
        GD.Print(ID);
    }
}
