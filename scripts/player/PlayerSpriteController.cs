using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class PlayerSpriteController : Skeleton2D
{

	// Yeah this script is kinda ass but it works for the alpha video
	// Feel free to completely rewrite, made by Joonatan
	AnimationPlayer animPlayer;

	float scaleForwards = 0.15f;
	float scaleBackwards = -0.15f;
	float animSpeedMultiplier = 200;

	[Export] bool isNpc;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void UpdateSprite(Vector2 movement)
	{
		if (movement.X != 0.0 || movement.Y != 0.0)
		{
			animPlayer.SpeedScale = movement.Length() / animSpeedMultiplier;
			animPlayer.Play("walk");

			if (movement.X >= 1.0)
			{
				Scale = new Vector2(scaleForwards, scaleForwards);
			}

			else if (movement.X <= -1.0)
			{
				Scale = new Vector2(scaleBackwards, scaleForwards);
			}

		}
		else
		{
			animPlayer.Play("idle");
		}
	}
}
