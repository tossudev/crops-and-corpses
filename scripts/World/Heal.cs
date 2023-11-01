using Godot;
using System;

public partial class Heal : Node
{
	[Export] public Item _healItem;
	[Export] public float _healAmount;

	[Export] public string _healMessage;


}
