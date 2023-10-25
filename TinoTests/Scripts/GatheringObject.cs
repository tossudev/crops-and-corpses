using Godot;
using System;
using Godot.Collections;

[GlobalClass, Icon("res://icon.svg")]
public partial class GatheringObject : Resource
{
    [Export] public int maxDrop { get; set; } = 2;
    [Export] public Array<Item> items { get; set; } = new Array<Item>();
}
