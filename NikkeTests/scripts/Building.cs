using Godot;
using System;

public partial class Building : Node
{
    public PackedScene scene;
    public PackedScene buildingModeScene;
    public int price;

    public Building(PackedScene scene, PackedScene buildingModeScene, int price)
    {
        this.scene = scene;
        this.buildingModeScene = buildingModeScene;
        this.price = price;
    }
}
