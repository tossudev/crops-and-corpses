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
		
		_parentScript?.CallDeferred("AttackReceived", attack);

		
		if (attack.effect == EffectType.Repair)
		{
			_healthComponent.TryHealWithRepairItem();
		}
		else
		{
			_healthComponent.TakeDamage(attack);
		}
	}

    public override void _Ready()
    {
        if (!SceneManager.IsCurrentScene(this, Scene.Town) && GetParent().IsInGroup("fence"))
        {
            QueueFree();
            return;
        }
    }
}
