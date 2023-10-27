using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Export] private Node _parentScript;
	[Export] private float _maxHealth = 100.0f;
	public float _health;

	public override void _Ready()
	{
		_health = _maxHealth;
	}

	public void TakeDamage(Attack attack)
	{
		_health -= attack.damage;

		GD.Print(GetParent().Name + " health: " + _health);

		if (_parentScript == null || !_parentScript.HasMethod("OnHealth"))
		{
			GD.Print("HealthComponent: No method or parent script found");
			return;
		}

		_parentScript.CallDeferred("OnHealth", _health);

		// if (_health <= 0)
		// {
		// 	if (GetParent().Name != "Player")
		// 	{
		// 		SpawnScript.RemoveZombieFromList(GetParent<CharacterBody2D>());
		// 		GD.Print("Check");
		// 	}

		// 	GetParent().QueueFree();
		// }
	}

	public float GetMaxHealth()
	{
		return _maxHealth;
	}
}
