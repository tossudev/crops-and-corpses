using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class LootController : StaticBody2D
{
	[Export] private Loot _loot;
	[Export] private AnimationPlayer _animationPlayer;

	private List<Item> _items = new List<Item>();

	public override void _Ready()
	{
		if (_loot == null)
		{
			GD.PrintErr("LootController: Loot is null");
			return;
		}

		foreach (var item in _loot.lootItems)
		{
			for (int i = 0; i < item.quantity; i++)
			{
				_items.Add(item.item);
			}
		}
	}

	private void OnHealth(float health)
	{
		if (_items.Count > 1)
			DropItems();

		_animationPlayer.SpeedScale = 15;
		_animationPlayer.Play("shake");

		if (health <= 0)
		{
			DropItems(_items.Count);
			QueueFree();
		}
	}

	private void DropItems(int dropAmount = 1)
	{
		for (int i = 0; i < dropAmount; i++)
		{
			int randIndex = (int)GD.RandRange(0, _items.Count - 1);

			RawInventoryItem dropItem = new RawInventoryItem(_items[randIndex].ID, _items[randIndex].Name, 1, _items[randIndex].StackSize);

			Node2D droppedItem = PlayerInventoryController.CreateDroppedItem(dropItem, GlobalPosition, GetParent());

			_items.RemoveAt(randIndex);

			MoveDropItem(droppedItem);
		}
	}

	private void MoveDropItem(Node2D droppedItem)
	{
		var tween = GetTree().CreateTween();
		tween.Parallel().TweenProperty(droppedItem, "position", droppedItem.Position + new Vector2(GD.RandRange(-75, 75), GD.RandRange(-75, 75)), 0.25f);
	}
}
