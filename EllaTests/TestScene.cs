using Godot;
using System;

public partial class TestScene : Node2D
{
	npcControl NPC;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		/* NPC = (npcControl)GD.Load<PackedScene>("res://EllaTests/npc.tscn").Instantiate();
		NPC.Position = new Vector2(500, 500);
		AddChild(NPC); */
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
