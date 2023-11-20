using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class LootController : StaticBody2D
{
	[Export] private Loot _loot;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _sprite;
	[Export] private Timer _timer;
	[Export] private Color _color;
	[Export] private float _minBrightness = 1f;

	private List<Item> _items = new List<Item>();
	private int animSpeed = 15;

	public override void _Ready()
	{
		// workaround for godot duplication reference thingy
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		_timer = GetNodeOrNull<Timer>("Timer");

		AppearanceVariation();

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

	private void AppearanceVariation()
	{
		float brightness = (float)GD.RandRange(1f, _minBrightness);

		if (_color != new Color(0, 0, 0, 0))
			_sprite.Modulate = new Color(_color.R * brightness, _color.G * brightness, _color.B * brightness, _color.A);

		if (GD.RandRange(0, 1) > 0.5f)
			Scale = new Vector2(-1, 1);

		Scale *= (float)GD.RandRange(0.75f, 1f);

		RotationDegrees = (float)GD.RandRange(-5, 5);
	}

	private void OnHealth(float health)
	{
		if (_items.Count > 1)
			DropItems();

		_animationPlayer.SpeedScale = animSpeed;
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
