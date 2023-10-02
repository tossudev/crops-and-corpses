using Godot;
using System;

public partial class InventoryItem : Node, ICraftable
{
    public bool isCraftable;

    public override void _Ready()
    {
        base._Ready();
        
        
    }
}