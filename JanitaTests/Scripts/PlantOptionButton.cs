using Godot;
using System;

public partial class PlantOptionButton : OptionButton
{
	[Export] FieldHandler _fh;

	private void _on_OptionButton_item_selected(int index)
	{
    	_fh.SetPlant(GetItemText(index));
	}
}
