using Godot;
using System;

public partial class ChangeScene : Area2D
{
	private string _areaName;
	private bool _playerInArea;
	
	public override void _Ready()
	{
		_playerInArea = false;
		_areaName = Name;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_playerInArea)
		{
			if(Input.IsActionJustPressed("interact"))
			{
				if (_areaName == "LevelOneToTown")
				{
					GetTree().ChangeSceneToFile("res://scenes/town.tscn");
				}
				if (_areaName == "TownToLevelOne")
				{
					// to be changed
					GetTree().ChangeSceneToFile("res://Liliantests/Test/level_one.tscn");
				}			
			}
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			_playerInArea = true;
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			_playerInArea = false;
		}
	}
}
