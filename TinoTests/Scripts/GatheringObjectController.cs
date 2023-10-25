using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class GatheringObjectController : StaticBody2D
{
	[Export] private GatheringObject _gatheringObject;
	[Export] private Sprite2D _sprite;

	private int _maxDrop;
	private List<Item> _items;

	public override void _Ready()
	{
		_maxDrop = _gatheringObject.maxDrop;
		_items = new List<Item>(_gatheringObject.items);
	}

	private void AttackReceived(Attack attack)
	{
	}

	private void OnHealth(float health)
	{
		if (health >= 0)
			DropItems();

		if (health <= 0)
		{
			_sprite.Visible = false;

			// very temp
			this.GetChild<CollisionShape2D>(2).Disabled = true;
		}
	}

	private void DropItems()
	{
		int dropAmount = (int)GD.RandRange(1, _gatheringObject.maxDrop);
		int count = 0;

		for (int i = 0; i < dropAmount; i++)
		{
			int randIndex = (int)GD.RandRange(0, _items.Count - 1);

			RawInventoryItem dropItem = new RawInventoryItem(_items[randIndex].ID, _items[randIndex].Name, 1, _items[randIndex].StackSize);

			// create drop item
			PlayerInventoryController.CreateDroppedItem(dropItem, Vector2.Zero, this);
			count++;
		}

		GD.Print("Dropped " + count + " items");
	}
}
