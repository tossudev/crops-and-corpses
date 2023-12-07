using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class LootController : StaticBody2D
{
	[Export] public Loot loot;
	[Export] private EffectType _requiredEffect = EffectType.None;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _sprite;
	[Export] private Color _color;
	//should be between 0 and 1
	[Export] private float _minBrightness = 1f;
	// should be between 0 and 180
	[Export] private int _rotationVariation = 0;
	//should be between 0 and 1
	[Export] private float _scaleVariation = 0f;
	[Export] private bool _flipVariation = true;

	[Export] private Node2D _dropLootPosition;
	[Export] private Node2D _fallingTreeBridge;

	private List<Item> _items = new List<Item>();
	RandomNumberGenerator rng;
	private int _meanDrop;
	static AudioController _audioController;
	private bool _canBeDestroyed;


	public override void _Ready()
	{
		if (loot == null)
			return;

		_canBeDestroyed = false;
		Init();
	}

	public void Init()
	{
		if (loot == null)
		{
			GD.PrintErr("LootController: Loot is null");
			return;
		}

		_meanDrop = loot.meanDrop;

		if (_dropLootPosition == null)
			_dropLootPosition = this;

		if (_sprite != null)
			Variations();

		_items.Clear();
		for (int i = 0; i < _meanDrop; i++)
		{
			_items.Add(loot.lootItems[GD.RandRange(0, loot.lootItems.Count - 1)].item);
		}

		if (GD.Randf() < 0.75f)
		{
			_items.RemoveAt(GD.RandRange(0, _items.Count - 1));
		}
		else if (GD.Randf() > 0.9f)
		{
			_items.Add(_items[GD.RandRange(0, _items.Count - 1)]);
		}
	}

	private void Variations()
	{
		rng = new RandomNumberGenerator();
		var x = (this.Position.X);
		var y = (this.Position.Y);
		rng.Seed = (ulong)((x + y) * (x + y + 1) / 2 + y);

		float brightness = rng.RandfRange(_minBrightness, 1f);

		if (_color != new Color(0, 0, 0, 0))
			_sprite.Modulate = new Color(_color.R * brightness, _color.G * brightness, _color.B * brightness, 1);

		if (_flipVariation && rng.RandfRange(0, 1) > 0.5f)
			this.Scale = new Vector2(-Scale.X, Scale.Y);

		this.Scale *= rng.RandfRange(1 - _scaleVariation, 1f);

		this.RotationDegrees += rng.RandfRange(-_rotationVariation, _rotationVariation);
	}

	private void AttackReceived(Attack attack)
	{
		if (attack.effect == _requiredEffect || _requiredEffect == EffectType.None || Name == "Backpack")
		{
			_canBeDestroyed = true;
		}
		else
		{
			_canBeDestroyed = false;
		}
	}

	private void OnHealth(float health)
	{
		if (!_canBeDestroyed)
			return;

		if (_items.Count > 1)
			DropItems();

		if (_animationPlayer != null)
		{
			_animationPlayer.SpeedScale = 10;
			_animationPlayer.Play("shake");
		}

		if (health <= 0)
		{
			if (Name == "FallingTree")
			{
				_animationPlayer?.Play("fall");
				TownManager.ApplyUnlock(TownUnlock.DIY_BRIDGE_UNLOCK);
			}
			else if (Name == "BridgeStalagmite")
			{
				_animationPlayer.SpeedScale = 2;
				_animationPlayer?.Play("fallingStalagmite");
				TownManager.ApplyUnlock(TownUnlock.STALAGMITE_UNLOCK);
			}
			else if (_fallingTreeBridge?.Name == "CaveBlockage")
			{
				TownManager.ApplyUnlock(TownUnlock.MINESHAFT_UNLOCK);
				DropItems(_items.Count);
				QueueFree();
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
		if (animationName == "fall" || animationName == "fallingStalagmite")
		{
			if (_fallingTreeBridge != null) _fallingTreeBridge.Visible = true;
			QueueFree();
		}
	}

	private void DropItems(int dropAmount = 1)
	{
		for (int i = 0; i < dropAmount; i++)
		{
			int randIndex = (int)GD.RandRange(0, _items.Count - 1);

			RawInventoryItem dropItem = new RawInventoryItem(_items[randIndex].ID, _items[randIndex].Name, 1, _items[randIndex].StackSize);

			Node2D droppedItem = PlayerInventoryController.CreateDroppedItem(dropItem, _dropLootPosition.GlobalPosition, GetParent().GetParent());

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
