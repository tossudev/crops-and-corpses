using Godot;
using System;
using System.Collections.Generic;

public partial class HealthComponent : Node2D
{
	[Export] private Node _parentScript;
	[Export] private float _maxHealth = 100.0f;
	public float health;

	[Export] PackedScene[] _healItemPrefabs;
	List<Heal> _heal = new List<Heal>();
	bool _isPlayer = false;
	int _count = 0;
	public override void _Ready()
	{
		health = _maxHealth;
		if (_parentScript != null && _parentScript.Name == "Player")
		{
			_isPlayer = true;
			InitializeHealItems();
		}
	}
	void InitializeHealItems()
	{
		for (int i = 0; i < _healItemPrefabs.Length; i++)
		{

			var scene = ResourceLoader.Load<PackedScene>(_healItemPrefabs[i].ResourcePath).Instantiate();
			Heal _newHeal = scene as Heal;
			if (_newHeal != null)
			{
				_heal.Add(_newHeal);
				GD.Print(_newHeal._healItem.Name);
			}
			else
			{
				GD.Print("Failed to cast to Heal: " + _healItemPrefabs[i].ResourceName);
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
		health -= attack.damage;

		GD.Print(GetParent().Name + " health: " + health);

		if (_parentScript == null || !_parentScript.HasMethod("OnHealth"))
		{
			GD.Print("HealthComponent: No method or parent script found");
			return;
		}

		_parentScript.CallDeferred("OnHealth", health);
	}

	public float GetMaxHealth()
	{
		return _maxHealth;
	}

	void Heal(float amount)
	{
		health += amount;
		if (health > _maxHealth) health = _maxHealth;
		PlayerInventoryController.RemoveItemFromInventory(new RawInventoryItem(
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
}
