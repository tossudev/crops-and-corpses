using Godot;
using System;

public partial class StaminaComponent : Node2D
{
	[Export] Timer _regenTimer;

	private int _maxStamina = 100;
	public int currentStamina;
	private ProgressBar _staminaBar;
	public bool canRegen = true;
	public bool canDrain = false;
	public float regenRate = 0.15f;
	public float drainRate = 0.25f;

	public override void _Ready()
	{
		_staminaBar = GetTree().GetFirstNodeInGroup("PlayerStaminaBar") as ProgressBar;
		_staminaBar.MaxValue = _maxStamina;
		currentStamina = _maxStamina;
		UpdateStaminaBar();
	}

	public override void _Process(double delta)
	{
		if (_regenTimer == null || _regenTimer.TimeLeft > 0)
		{
			return;
		}

		if (currentStamina <= 0)
		{
			canRegen = true;
			canDrain = false;
		}
		else if (currentStamina >= _maxStamina)
		{
			canRegen = false;
		}
		else
		{
			canRegen = true;
		}

		if (canDrain == true)
		{
			canRegen = false;
		}

		if (canRegen && currentStamina < _maxStamina)
		{
			RegenStamina(regenRate);
			UpdateStaminaBar();
		}
		else if (canDrain && currentStamina > 0)
		{
			DrainStamina(drainRate);
			UpdateStaminaBar();
		}
	}

	private void RegenStamina(float rate)
	{
		_regenTimer.Start(rate);
		currentStamina += 1;

		if (currentStamina > _maxStamina)
		{
			currentStamina = _maxStamina;
		}
	}

	private void DrainStamina(float rate)
	{
		_regenTimer.Start(rate);
		currentStamina -= 1;

		if (currentStamina < 0)
		{
			currentStamina = 0;
		}
	}

	public void UseStamina(int amount)
	{
		currentStamina -= amount;

		if (currentStamina < 0)
		{
			currentStamina = 0;
		}

		UpdateStaminaBar();
	}

	private void StaminaCooldown(float duration)
	{
		_regenTimer.Start(duration);
	}

	private void UpdateStaminaBar()
	{
		if (_staminaBar == null)
		{
			return;
		}

		_staminaBar.Value = currentStamina;
		_staminaBar.GetNodeOrNull<Label>("Text").Text = currentStamina.ToString();
	}
}
