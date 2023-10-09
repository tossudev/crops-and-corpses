using Godot;
using System;

public partial class PlantOptionButton : OptionButton
{
	string seed;
	private void _on_OptionButton_item_selected(int index)
	{
    	seed = GetItemText(index);
	}

	public string GetSeedFromOption(){
		return seed;
	}
}
