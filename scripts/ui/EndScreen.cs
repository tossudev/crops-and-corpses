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
		Visible = false;
	}	
	public override void _Process(double delta)
	{
		if (TownManager.EveryXSecond(7))
		{
			if (TownManager.currentTownStats.townHallLevel > 4)
			{
				if (!TownManager.currentTownStats.gameFinished)
				{
					TownManager.currentTownStats.gameFinished = true;
					StartEndTextAnimation();
				}
			}
		}
	}

	Timer _endScreenTimer;
	void StartEndTextAnimation()
	{
		Visible = true;
		
		GetTree().Paused = true;
		
		_endScreenTimer = new Timer()
		{
			Autostart = true,
			OneShot = true,
			WaitTime = 20f
		};

		_endScreenTimer.Timeout += ContinueGame;
		AddChild(_endScreenTimer);
		
		_animationPlayer.Play("EndingTextMovement");
	}

	void ContinueGame()
	{
		Visible = false;
		_animationPlayer.Stop();
		GetTree().Paused = false;
	}
}
