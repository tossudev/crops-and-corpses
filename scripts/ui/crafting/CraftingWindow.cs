using Godot;
using System;
using System.Collections.Generic;


public partial class CraftingWindow : Control
{
	Panel _itemArea; 
	bool _isOpen;

	CraftPanel _craftPanel;
	const string CRAFT_PANEL_NODENAME = "%CraftPanel";
	
	public override void _Ready()
	{
		_itemArea = GetNode<Panel>("ItemArea");
		_craftPanel = GetNode<CraftPanel>(CRAFT_PANEL_NODENAME);
		
		_itemArea.Visible = false;
		_isOpen = false;
	}
    
	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("toggle_crafting_window")) {
			_isOpen = !_isOpen;
			_itemArea.Visible = _isOpen;

			if (!_itemArea.Visible)
			{
				_craftPanel.ClosePanel();
			}
		}
		
		if (@event.IsActionPressed("close_crafting_window"))
		{
			if (!_isOpen) return;
			
			_isOpen = false;
			_itemArea.Visible = false;
			_craftPanel.ClosePanel();
		}
	}
}
