using Godot;
using System;

public partial class HitboxComponent : Area2D
{
	[Export] private HealthComponent _healthComponent;

	public void Damage(Attack attack)
	{
		if (_healthComponent != null)
		{
			_healthComponent.TakeDamage(attack);
		}
		else
		{
			GD.Print("No health component found");
		}
	}
}
