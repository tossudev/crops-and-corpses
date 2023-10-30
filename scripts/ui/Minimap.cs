using Godot;
using System;

public partial class Minimap : Control {

	public CharacterBody2D player;
	public Camera2D cam;
	public PackedScene markerNode;
	public const int MAP_SIZE_DIVIDER = 8;

	public override void _Ready() {
		foreach (CharacterBody2D playerNode in GetTree().GetNodesInGroup("player")) {
			player = playerNode;
		}

		cam = GetNode<Camera2D>("SubViewport/Radar/Camera");
		markerNode = GD.Load<PackedScene>("res://scenes/ui/minimap_marker.tscn");

		foreach (CharacterBody2D npc in GetTree().GetNodesInGroup("npc")) {
			Sprite2D newNpc = markerNode.Instantiate<Sprite2D>();
			newNpc.GlobalPosition = npc.GlobalPosition / MAP_SIZE_DIVIDER;
			cam.AddChild(newNpc);
		}
	}

	public override void _Process(double delta) {
		cam.GlobalPosition = player.GlobalPosition / MAP_SIZE_DIVIDER;
	}
}
