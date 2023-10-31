using Godot;
using System;

public partial class Minimap : Control {

	public CharacterBody2D player;
	public Node2D radar;
	public Camera2D cam;
	public PackedScene markerNode;
	public const int MAP_SIZE_DIVIDER = 10;

	Texture2D _characterTexture;
	Texture2D _decoTexture;

	public override void _Ready() {
		foreach (CharacterBody2D playerNode in GetTree().GetNodesInGroup("player")) {
			player = playerNode;
		}

		cam = GetNode<Camera2D>("SubViewport/Radar/Camera");
		radar = GetNode<Node2D>("SubViewport/Radar");

		markerNode = GD.Load<PackedScene>("res://scenes/ui/minimap_marker.tscn");
		_characterTexture = GD.Load<Texture2D>("res://assets/placeholder/test_character/character.png");
		_decoTexture = GD.Load<Texture2D>("res://assets/sprites/foliage/sprite_tree1.png");

		foreach (CharacterBody2D npc in GetTree().GetNodesInGroup("npc")) {
			Sprite2D newNpcMarker = markerNode.Instantiate<Sprite2D>();
			newNpcMarker.GlobalPosition = npc.GlobalPosition / MAP_SIZE_DIVIDER;
			newNpcMarker.Texture = _characterTexture;
			radar.AddChild(newNpcMarker);
		}

		// Change the path later
		foreach (Node2D decoNode in GetNode("../../../Objects/Deco").GetChildren()) {
			Sprite2D newObjectMarker = markerNode.Instantiate<Sprite2D>();
			newObjectMarker.GlobalPosition = decoNode.GlobalPosition / MAP_SIZE_DIVIDER;
			newObjectMarker.Texture = _decoTexture;
			radar.AddChild(newObjectMarker);
		}
	}

	public override void _Process(double delta) {
		cam.GlobalPosition = player.GlobalPosition / MAP_SIZE_DIVIDER;
	}
}
