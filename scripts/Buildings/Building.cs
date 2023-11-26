using Godot;
using System;

public partial class Building : Node
{
    public PackedScene scene;
    public PackedScene buildingModeScene;
    public Texture2D icon;
    public int price;
    public string name;

    public Building(PackedScene scene, PackedScene buildingModeScene, int price, string name, Texture2D icon)
    {
        this.scene = scene;
        this.buildingModeScene = buildingModeScene;
        this.price = price;
        this.name = name;
        this.icon = icon;
    }
}
