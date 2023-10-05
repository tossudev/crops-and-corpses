using Godot;
using System;

public partial class InteractionDummy : StaticBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Hello from InteractionDummy");
	}

	private void OnInteractable(Area2D body)
	{
		GD.Print("Interactable");
	}
}
