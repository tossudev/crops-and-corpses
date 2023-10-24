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

	private void AttackReceived(Attack attack)
	{
		DropItems();
	}

	private void DropItems()
	{
		int dropAmount = (int)GD.RandRange(1, _gatheringObject.maxDrop);

		List<Item> itemsToDrop = new List<Item>();

		for (int i = 0; i < dropAmount; i++)
		{
			int randomIndex = (int)GD.RandRange(0, _items.Count - 1);
			itemsToDrop.Add(_items[randomIndex]);
		}

		// drop items

	}
}
