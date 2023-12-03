using Godot;
using System;
using System.Threading.Tasks;

public partial class Minimap : Control {

	public CharacterBody2D player;
	public Node2D radar;
	public Camera2D cam;
	public PackedScene markerNode;
	public TileMap groundTiles;
	public TileMap waterTiles;
	public Node fences;
	public const int MAP_SIZE_DIVIDER = 10;

	Texture2D _characterTexture;
	Texture2D _decoTexture;

	const string FOLIAGE_GROUP = "Foliage";

	public override void _Ready()
	{
		return;
		player = (CharacterBody2D) GetTree().GetFirstNodeInGroup("player");

		cam = GetNode<Camera2D>("SubViewport/Radar/Camera");
		radar = GetNode<Node2D>("SubViewport/Radar");

		markerNode = GD.Load<PackedScene>("res://scenes/ui/minimap_marker.tscn");
		_characterTexture = GD.Load<Texture2D>("res://assets/placeholder/test_character/character.png");
		_decoTexture = GD.Load<Texture2D>("res://assets/sprites/foliage/sprite_tree1.png");

		

		// Change the path later
		foreach (Node2D decoNode in GetTree().GetNodesInGroup(FOLIAGE_GROUP)) {
			Sprite2D newObjectMarker = markerNode.Instantiate<Sprite2D>();
			newObjectMarker.GlobalPosition = decoNode.GlobalPosition / MAP_SIZE_DIVIDER;
			newObjectMarker.Texture = _decoTexture;
			radar.AddChild(newObjectMarker);
		}
		
		var root = GetTree().Root;

		groundTiles = root.FindChild("GroundTiles") as TileMap;
		waterTiles = root.FindChild("TempWater") as TileMap;
		fences = root.FindChild("Fences") as Node2D;
			
		CreateMap();
	}
	
	public override void _Process(double delta) {
		//cam.GlobalPosition = player.GlobalPosition / MAP_SIZE_DIVIDER;
	}


	void AddVillagers()
	{
		foreach (CharacterBody2D npc in GetTree().GetNodesInGroup("npc")) {
			Sprite2D newNpcMarker = markerNode.Instantiate<Sprite2D>();
			newNpcMarker.GlobalPosition = npc.GlobalPosition / MAP_SIZE_DIVIDER;
			newNpcMarker.Texture = _characterTexture;
			radar.AddChild(newNpcMarker);
		}
	}

	void CreateMap() {
		Node mapTiles = groundTiles.Duplicate();
		Node mapWaterTiles = waterTiles.Duplicate();
		Node fenceTiles = fences.Duplicate();
		
		radar.AddChild(mapTiles);
		radar.AddChild(fenceTiles);
		radar.AddChild(mapWaterTiles);
		
		TileMap tileNode = mapTiles as TileMap;
		tileNode.Scale = new Vector2(1f/MAP_SIZE_DIVIDER, 1f/MAP_SIZE_DIVIDER);
		
		TileMap watertileNode = mapWaterTiles as TileMap;
		watertileNode.Scale = new Vector2(1f/MAP_SIZE_DIVIDER, 1f/MAP_SIZE_DIVIDER);

		Node2D fenceNode = fenceTiles as Node2D;
		fenceNode.Scale = new Vector2(1f/MAP_SIZE_DIVIDER, 1f/MAP_SIZE_DIVIDER);

	}
}
