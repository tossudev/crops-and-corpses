using Godot;
using System;

public partial class DaniTest : Sprite2D
{
[Export]

	private Vector2 position;
	// Called when the node enters the scene tree for the first time.


		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("Click"))
		{
			position = GetGlobalMousePosition();
			Position = position;
			
		}
		}
	
}
