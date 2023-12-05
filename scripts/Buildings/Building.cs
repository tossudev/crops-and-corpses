using Godot;
using System;

public partial class Building : Node
{
    public PackedScene scene;
    public PackedScene buildingModeScene;
    public Texture2D icon;
    public int priceLogs;
    public int priceStone;
    public string name;
    public ExpGain buildingExp;

    public Building(PackedScene scene, PackedScene buildingModeScene, int priceLogs, int priceStone, ExpGain buildingExp, string name, Texture2D icon)
    {
        this.scene = scene;
        this.buildingModeScene = buildingModeScene;
        this.priceLogs = priceLogs;
        this.priceStone = priceStone;
        this.buildingExp = buildingExp;
        this.name = name;
        this.icon = icon;
    }
}
