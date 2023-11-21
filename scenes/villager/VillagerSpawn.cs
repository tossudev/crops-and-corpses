using Godot;
using System;

public partial class VillagerSpawn : Node2D
{
	NodePath rootPath;
	Node2D rootNode;
	Node2D _villagerSpawnPoint;
	PackedScene _villagerPackedScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_villagerPackedScene = (PackedScene)GD.Load("res://scenes/villager/villager.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void SpawnVillager()
	{
		rootPath = GetParent<Node2D>().GetPath();
		rootNode = GetNodeOrNull<Node2D>(rootPath);

		CharacterBody2D _villagerPrefab = (CharacterBody2D)_villagerPackedScene.Instantiate();
		_villagerPrefab.Position = _villagerSpawnPoint.Position;
		rootNode.AddChild(_villagerPrefab);
	}
}
