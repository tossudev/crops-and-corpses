using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class LootController : StaticBody2D
{
	[Export] private Loot _loot;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _sprite;
	[Export] private Color _color;
	//should be between 0 and 1
	[Export] private float _minBrightness = 1f;
	// should be between 0 and 180
	[Export] private int _rotationVariation = 0;
	//should be between 0 and 1
	[Export] private float _scaleVariation = 0f;

	private List<Item> _items = new List<Item>();
	RandomNumberGenerator rng;

	public override void _Ready()
	{
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

		rng = new RandomNumberGenerator();
		rng.Seed = this.GetInstanceId();

		Variations();

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

	private void Variations()
	{
		float brightness = rng.RandfRange(_minBrightness, 1f);

		if (_color != new Color(0, 0, 0, 0))
			_sprite.Modulate = new Color(_color.R * brightness, _color.G * brightness, _color.B * brightness, 1);

		if (rng.RandfRange(0, 1) > 0.5f)
			this.Scale = new Vector2(-Scale.X, Scale.Y);

		this.Scale *= rng.RandfRange(1 - _scaleVariation, 1f);

		this.RotationDegrees += rng.RandfRange(-_rotationVariation, _rotationVariation);
	}

	private void OnHealth(float health)
	{
		if (_items.Count > 1)
			DropItems();

		_animationPlayer.SpeedScale = 10;
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

			Node2D droppedItem = PlayerInventoryController.CreateDroppedItem(dropItem, this.Position, GetParent());

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
