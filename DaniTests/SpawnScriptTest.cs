using Godot;
using System;

public partial class SpawnScriptTest : Node2D
{

	PackedScene packedScene;

	public override void _Ready()
	{
		packedScene = (PackedScene)GD.Load("res://DaniTests/PrefabTest.tscn");
	}
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventMouseButton mouseEvent)
		{
			CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
			prefab.Position = mouseEvent.Position;
			this.AddChild(prefab);
		}
    }
}
