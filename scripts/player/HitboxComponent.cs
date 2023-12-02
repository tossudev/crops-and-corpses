using Godot;
using System;

public partial class HitboxComponent : Area2D
{
	[Export] private Node _parentScript;
	[Export] private HealthComponent _healthComponent;

	public void ApplyAttack(Attack attack)
	{
		if (_healthComponent == null)
		{
			// GD.PrintErr("HitboxComponent: No health component found");
			return;
		}

		if (_parentScript == null || !_parentScript.HasMethod("AttackReceived"))
		{
			// GD.Print("HitboxComponent: No method or parent script found");

			// temp
			if (this.GetParent().HasMethod("AttackReceived"))
				this.GetParent().CallDeferred("AttackReceived", attack);
			//

			return;
		}

		_parentScript.CallDeferred("AttackReceived", attack);

		if (attack.effect == EffectType.Repair)
		{
			_healthComponent.TryHealWithRepairItem();
			return;
		}
		else
		{
			_healthComponent.TakeDamage(attack);
		}
	}
}
