using Godot;
using System;

public partial class ZombieStates : Node
{
	[Signal] public delegate void TransitionedEventHandler(string myString);
	public virtual void Enter()
	{		
	}

	public virtual void Exit()
	{
	}
	
	public virtual void Update(double delta)
	{
	}
		
	public virtual void Physics_Update(double delta)
	{
	}
}
