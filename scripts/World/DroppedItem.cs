using Godot;
using System;

public partial class DroppedItem : Node2D
{
	RawInventoryItem _containedRawItem;
	Item _containedAsItem;
	[Export] TextureRect _displayImage;

	public void SetItem(RawInventoryItem item)
	{
		_containedRawItem = new RawInventoryItem(item.id, item.name, item.quantity, item.stackSize);
		_containedAsItem = ItemData.GetItemById(_containedRawItem.id);

		_displayImage.Texture = _containedAsItem.IconTexture;
	}

	public void Pickup()
	{
		int addedItems = PlayerInventoryController.AddItem(_containedRawItem, -1, true, false);

		if (addedItems > 0) return;
		
		QueueFree();
	}
}
