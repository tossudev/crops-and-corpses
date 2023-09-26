using Godot;
using System;

public partial class Ineractabe : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnAreaEntered(CollisionObject2D body)
	{
		GD.Print("Interactable: I'm interacted!");
	}

	private void OnAreaExited(CollisionObject2D body)
	{
		GD.Print("Interactable: No more interaction :(");
	}
}
