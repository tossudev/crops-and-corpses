using Godot;
using System;

public partial class CraftingWindow : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		isOpen = false;
	}

	bool isOpen;
	
	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("toggle_crafting_window")) {
			isOpen = !isOpen;
			Visible = isOpen;
		}
	}
}
