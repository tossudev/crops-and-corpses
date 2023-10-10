using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Export] private float _maxHealth = 100.0f;
	private float _health;

	public override void _Ready()
	{
		_health = _maxHealth;
	}

	public void TakeDamage(Attack attack)
	{
		_health -= attack.damage;

		GD.Print("Health: " + _health);

		if (_health <= 0)
		{
			GetParent().QueueFree();
		}
	}

	public void Heal(Attack attack)
	{
		_health += attack.damage;

		if (_health > _maxHealth)
		{
			_health = _maxHealth;
		}
	}

	public float GetHealth()
	{
		return _health;
	}

	public float GetMaxHealth()
	{
		return _maxHealth;
	}

}
