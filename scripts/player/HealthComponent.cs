using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

public partial class HealthComponent : Node2D
{
	[Export] private Node _parentScript;
	[Export] private int _maxHealth = 100;
	int _health;

	[Export] bool _hasHealthBar = true;
	[Export] bool _hideHealthBarOnFullHP = true;
	ProgressBar _healthBar;
	const string HEALTH_BAR_NODENAME = "%HealthBar";

	Node _overlay;
	const string OVERLAY_NODENAME = "%PlayerOverlay";


	[Export] PackedScene[] _healItemPrefabs;
	List<Heal> _heal = new List<Heal>();
	bool _isPlayer = false;

	int _count = 0;

	public override void _Ready()
	{
		_health = _maxHealth;

		if (_parentScript != null && _parentScript.Name == "Player")
		{
			_isPlayer = true;
			InitializeHealItems();
		}

		if (_hasHealthBar)
		{
			_healthBar = _isPlayer
				? GetTree().GetFirstNodeInGroup("PlayerHealthBar") as ProgressBar
				: GetParent().GetNodeOrNull<ProgressBar>(HEALTH_BAR_NODENAME);

			if (_healthBar == null)
			{
				GD.Print("Error: HealthBar not found: " + GetParent().Name);
				return;
			}
			_healthBar.MaxValue = _maxHealth;
			_healthBar.Value = _health;
			UpdateHealthBar();
		}
	}


	void InitializeHealItems()
	{
		foreach (var packedScene in _healItemPrefabs)
		{
			var scene = ResourceLoader.Load<PackedScene>(packedScene.ResourcePath).Instantiate();

			if (scene is Heal _newHeal)
			{
				_heal.Add(_newHeal);
				GD.Print(_newHeal._healItem.Name);
			}

			else
			{
				GD.Print("Failed to cast to Heal: " + packedScene.ResourceName);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isPlayer || PlayerInventoryController.selectedItem == null)
		{
			_count = 0;
			return;
		}
		if (Input.IsActionJustPressed("Click"))
		{
			_count++;
			if (_count == 2)
			{
				TryHeal();
			}

		}


	}
	public void TakeDamage(Attack attack)
	{
		_health -= attack.damage;
		UpdateHealthBar();

		if (_parentScript == null || !_parentScript.HasMethod("OnHealth"))
		{
			// GD.Print("HealthComponent: No method or parent script found");
			return;
		}

		_parentScript.CallDeferred("OnHealth", _health);
	}

	public int GetMaxHealth()
	{
		return _maxHealth;
	}

	async void Heal(int amount)
	{
		_health += amount;
		UpdateHealthBar();
		if (_health > _maxHealth) _health = _maxHealth;
		await PlayerInventoryController.RemoveItemFromInventory(new RawInventoryItem(
			PlayerInventoryController.selectedItem.id,
			PlayerInventoryController.selectedItem.name,
			1,
			PlayerInventoryController.selectedItem.stackSize));
	}
	void TryHeal()
	{
		foreach (Heal h in _heal)
		{
			if (h._healItem.Name == PlayerInventoryController.selectedItem.name)
			{
				Heal(h._healAmount);
				GD.Print(h._healMessage);
				_count = 0;
				return;
			}
		}
	}

	public void UpdateHealthBar()
	{
		if (!_hasHealthBar) return;

		if (_healthBar == null)
		{
			GD.PushError("HealthBar missing from " + GetParent().Name);
			return;
		}

		_healthBar.Value = _health;

		if (_hideHealthBarOnFullHP)
		{
			_healthBar.Visible = _health != _maxHealth;
		}

		if (_isPlayer)
		{
			_healthBar.GetNode<Label>("%HealthText").Text = _health.ToString();
		}
	}

	public void SetHealth(int health)
	{
		_health = health;
		UpdateHealthBar();
	}
}
