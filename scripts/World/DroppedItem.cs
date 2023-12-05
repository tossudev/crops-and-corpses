using Godot;
using System;

public partial class DroppedItem : Node2D
{
	RawInventoryItem _containedRawItem;
	Item _containedAsItem;
	[Export] TextureRect _displayImage;
	static AudioController _audioController;



    public override void _Ready() {
		_audioController = GetNode<AudioController>("/root/Audio");
    }

    public void SetItem(RawInventoryItem item)
	{
		_containedRawItem = new RawInventoryItem(item.id, item.name, item.quantity, item.stackSize);
		_containedAsItem = ItemData.GetItemById(_containedRawItem.id);

		_displayImage.Texture = _containedAsItem.IconTexture;
	}

	public async void Pickup()
	{
		int addedItems = await PlayerInventoryController.AddItemToHotbarOrInventory(_containedRawItem, -1, true, false);

		if (addedItems > 0) return;
		
		_audioController.PlayEffect("res://assets/Sounds/character_sounds/pickup_item.wav");
		QueueFree();
	}
}
