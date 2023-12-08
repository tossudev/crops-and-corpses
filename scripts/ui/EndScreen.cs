using Godot;
using System;

public partial class EndScreen : Node2D
{
	const string ANIMATION_PLAYER = "%AnimationPlayer";
	AnimationPlayer _animationPlayer;
	AnimationPlayer _villagerAnimation;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>(ANIMATION_PLAYER);
		StartEndTextAnimation();	
	}	
	public override void _Process(double delta)
	{

	}

	void StartEndTextAnimation()
	{
		_animationPlayer.Play("EndingTextMovement");
	}
}
