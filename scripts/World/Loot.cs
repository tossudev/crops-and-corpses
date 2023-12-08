using Godot;
using System;
using Godot.Collections;

[GlobalClass, Icon("res://icon.svg")]
public partial class Loot : Resource
{
    [Export] public Array<CraftingRequirement> lootItems { get; set; } = new Array<CraftingRequirement>();
    [Export] public int meanDrop { get; set; } = 1;
    [Export] public ExpGain expGain { get; set; }
}
