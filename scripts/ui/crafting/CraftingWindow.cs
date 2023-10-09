using Godot;
using System;

public partial class CraftingWindow : Control
{
	TextureRect _itemArea; 
	bool _isOpen;

	
	public override void _Ready()
	{
		_itemArea = GetNode<TextureRect>("ItemArea");
		
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
