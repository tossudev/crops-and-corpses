using Godot;
using System;

public partial class CraftingWindow : Control
{
	Panel _itemArea; 
	bool _isOpen;

	
	public override void _Ready()
	{
		_itemArea = GetNode<Panel>("ItemArea");
		
		_itemArea.Visible = false;
		_isOpen = false;
	}
    
	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("toggle_crafting_window")) {
			_isOpen = !_isOpen;
			_itemArea.Visible = _isOpen;
		}
	}
}
