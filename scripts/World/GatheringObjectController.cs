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
	private float healthPercent;

	public override void _Ready()
	{
		_maxDrop = _gatheringObject.maxDrop;
		_items = new List<Item>(_gatheringObject.items);
	}

	private void OnHealth(float health)
	{
		DropItems();

		if (health <= 0)
		{
			QueueFree();
		}
	}

	private void DropItems()
	{
		int weight = 3;
		int dropAmount = GD.RandRange(0, weight) == 0 ? GD.RandRange(1, _maxDrop) : 1;

		for (int i = 0; i < dropAmount; i++)
		{
			int randIndex = (int)GD.RandRange(0, _items.Count - 1);

			RawInventoryItem dropItem = new RawInventoryItem(_items[randIndex].ID, _items[randIndex].Name, 1, _items[randIndex].StackSize);

			Node2D droppedItem = PlayerInventoryController.CreateDroppedItem(dropItem, GlobalPosition, GetParent());

			MoveDropItem(droppedItem);
		}
	}

	private void MoveDropItem(Node2D droppedItem)
	{
		var tween = GetTree().CreateTween();
		tween.Parallel().TweenProperty(droppedItem, "position", droppedItem.Position + new Vector2(GD.RandRange(-75, 75), GD.RandRange(-75, 75)), 0.25f);
	}
}
