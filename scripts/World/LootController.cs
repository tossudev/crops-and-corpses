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

	[Export] private Node2D _dropLootPosition;
	[Export] private Node _dropLoopParent;

	private List<Item> _items = new List<Item>();
	RandomNumberGenerator rng;
	private int _meanDrop;

	public override void _Ready()
	{
		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		_animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		_meanDrop = _loot.meanDrop;

		if (_dropLootPosition == null)
			_dropLootPosition = this;

		_dropLoopParent = GetNodeOrNull("../../Objects");

		if (_dropLoopParent == null)
			_dropLoopParent = GetParent();

		rng = new RandomNumberGenerator();
		rng.Seed = this.GetInstanceId();

		Variations();

		if (_loot == null)
		{
			GD.PrintErr("LootController: Loot is null");
			return;
		}

		for (int i = 0; i < _meanDrop; i++)
		{
			_items.Add(_loot.lootItems[GD.RandRange(0, _loot.lootItems.Count - 1)].item);
		}

		if (GD.Randf() < 0.75f)
		{
			if (GD.Randf() < 0.75f)
			{
				_items.RemoveAt(GD.RandRange(0, _items.Count - 1));
			}
			else
			{
				_items.Add(_items[GD.RandRange(0, _items.Count - 1)]);
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
		if (_items.Count - (_items.Count - _meanDrop) > 1)
			DropItems();

		if (_animationPlayer != null)
		{
			_animationPlayer.SpeedScale = 10;
			_animationPlayer.Play("shake");
		}

		if (health <= 0)
		{
			if (Name == "TreeBridge")
			{
				_animationPlayer.Play("fall");
			}
			else
			{
				DropItems(_items.Count);
				QueueFree();
			}
		}
	}

	private void OnAnimationFinished(string animationName)
	{
		if (animationName == "fall")
		{
			QueueFree();
		}
	}

	private void DropItems(int dropAmount = 1)
	{
		for (int i = 0; i < dropAmount; i++)
		{
			int randIndex = (int)GD.RandRange(0, _items.Count - 1);

			RawInventoryItem dropItem = new RawInventoryItem(_items[randIndex].ID, _items[randIndex].Name, 1, _items[randIndex].StackSize);

			Node2D droppedItem = PlayerInventoryController.CreateDroppedItem(dropItem, _dropLootPosition.Position, _dropLoopParent);

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
