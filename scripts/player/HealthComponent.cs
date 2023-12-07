using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;

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


	BuildingHealth _building;
	bool _isBuilding;

	[Export] PackedScene[] _healItemPrefabs;
	List<Heal> _heal = new List<Heal>();
	bool _isPlayer = false;

	int _count = 0;

	public override async void _Ready()
	{
		if (_parentScript != null && _parentScript.Name == "Player")
		{
			_isPlayer = true;
		}
		InitializeHealItems();

		_health = _maxHealth;

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

		if (_isBuilding)
		{
           
            if (_building.buildingType == BuildingType.House && _building.isLoaded)
            {
                await Task.Delay((int)(GD.Randi() % 1000));

                SetMaxHealth(_maxHealth + TownManager.currentTownStats.houseHP);
				_building.LoadBuildingHealth(_building.loadedHealth);
            }
            else if (_building.buildingType == BuildingType.House)
            {
                SetMaxHealth(_maxHealth + TownManager.currentTownStats.houseHP);
                SetHealth(_maxHealth);
            }
            else if (_building.buildingType == BuildingType.Fence)
            {
                SetMaxHealth(_maxHealth + TownManager.currentTownStats.wallHP);
                UpdateHealthBar();
            }
        }

		if (_isPlayer)
		{
			_health = await PlayerInfo.GetHealth();
			if (_health <= 0) _health = _maxHealth;
			UpdateHealthBar();
		}
	}

	void InitializeHealItems()
	{
		if (_healItemPrefabs == null) return;

		foreach (var packedScene in _healItemPrefabs)
		{
			var scene = ((PackedScene)FileLoader.LoadCustomResource(packedScene.ResourcePath)).Instantiate();

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

	public void AssignBuilding(BuildingHealth building)
	{
		_building = building;
		_isBuilding = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (PlayerInventoryController.selectedItem == null)
		{
			_count = 0;
			return;
		}
		if (Input.IsActionJustPressed("Click"))
		{
			_count++;
			if (_count == 2)
			{
				TryHealWithItem();
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

	void Heal(int amount)
	{
		_health += amount;
		if (_health > _maxHealth) _health = _maxHealth;
		UpdateHealthBar();
	}

	public async void TryHealWithRepairItem()
	{
		if (!_isBuilding) return;

		RawInventoryItem itemToRemove = null;
		if (PlayerInventoryController.isItemSelected)
		{
			itemToRemove = PlayerInventoryController.selectedItem;
		}
		else if (PlayerInventoryController.heldItem != null)
		{
			itemToRemove = PlayerInventoryController.heldItem;
			if (itemToRemove.quantity == 0) return;
		}

		if (itemToRemove == null)
		{
			GD.PushError("Can't heal with null item");
			return;
		}

		if (_health == _maxHealth) return;

		Heal(_maxHealth / 2);

		await PlayerInventoryController.RemoveItemFromInventory(new RawInventoryItem(
			itemToRemove.id,
			itemToRemove.name,
			1,
			itemToRemove.stackSize),
			itemToRemove.indexInStorageArray);

		_parentScript.CallDeferred("OnHealth", _health);
	}


	public async void TryHealWithItem()
	{
		foreach (Heal h in _heal)
		{
			if (h._healItem.Name == PlayerInventoryController.selectedItem.name)
			{
				Heal(h._healAmount);

				await PlayerInventoryController.RemoveItemFromInventory(new RawInventoryItem(
					PlayerInventoryController.selectedItem.id,
					PlayerInventoryController.selectedItem.name,
					1,
					PlayerInventoryController.selectedItem.stackSize));

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
		_parentScript.CallDeferred("OnHealth", _health);
		UpdateHealthBar();
	}
	
	public void SetMaxHealth(int health)
	{
		_maxHealth = health;
        _healthBar.MaxValue = _maxHealth;

        if (_health > _maxHealth)
		{
			SetHealth(_maxHealth);
		}
	}

	public int GetHealth()
	{
		return _health;
	}
}
