using Godot;
using System;

public partial class HitboxComponent : Area2D
{
	[Export] private HealthComponent _healthComponent;
	[Export] private bool _knockback = false;

	public void ApplyAttack(Attack attack)
	{
		if (_healthComponent != null)
		{
			_healthComponent.TakeDamage(attack);

			if (_knockback)
				ApplyKnockback(attack);
		}
		else
		{
			GD.Print("No health component found");
		}
	}

	private void ApplyKnockback(Attack attack)
	{
		var direction = attack.direction;
		var knockback = attack.knockback;
		var velocity = direction * knockback;

		CharacterBody2D parent = this.GetParent() as CharacterBody2D;
		// give knockback to parent
	}
}
