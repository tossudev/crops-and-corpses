using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class GatheringObjectController : StaticBody2D
{
	[Export] private GatheringObject _gatheringObject;

	private int _maxDrop;
	private List<Item> _items;

	public override void _Ready()
	{
		_maxDrop = _gatheringObject.maxDrop;
		_items = new List<Item>(_gatheringObject.items);
	}

	private void OnHealth(float health)
	{
		if (health >= 0)
			DropItems();

		if (health <= 0)
		{
			QueueFree();
		}
	}

	private void DropItems()
	{
		int dropAmount = (int)GD.RandRange(1, _gatheringObject.maxDrop);

		for (int i = 0; i < dropAmount; i++)
		{
			int randIndex = (int)GD.RandRange(0, _items.Count - 1);

			RawInventoryItem dropItem = new RawInventoryItem(_items[randIndex].ID, _items[randIndex].Name, 1, _items[randIndex].StackSize);

			Node2D droppedIrem = PlayerInventoryController.CreateDroppedItem(dropItem, GlobalPosition, GetParent());

			MoveDropItem(droppedIrem);
		}
	}

	private void MoveDropItem(Node2D droppedItem)
	{
		var tween = GetTree().CreateTween();
		tween.Parallel().TweenProperty(droppedItem, "position", droppedItem.Position + new Vector2(GD.RandRange(-75, 75), GD.RandRange(-75, 75)), 0.25f);
	}
}
