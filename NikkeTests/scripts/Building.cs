using Godot;
using System;

public partial class Building : Node
{
    public PackedScene scene;
    public int price;

    public Building(PackedScene scene, int price)
    {
        this.scene = scene;
        this.price = price;
    }

}
