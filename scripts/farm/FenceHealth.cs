using Godot;
using System;

public partial class FenceHealth : Node2D
{
	private void OnHealth(float _health)
	{
		if (_health <= 0)
		{
			GetParent().QueueFree();
		}
	}

	private void AttackReceived(Attack attack)
	{
		// idk made this just to get rid of the debug spam 
	}
}
