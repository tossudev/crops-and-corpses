using Godot;
using System;

[GlobalClass, Icon("res://icon.svg")]
public partial class CraftingRequirement : Resource
{
    [Export] public Item item;
    [Export] public int quantity { get; set; }

    public RawInventoryItem RequirementAsRaw()
    {
        return new RawInventoryItem(item.ID, item.Name, quantity, item.StackSize);
    }
}