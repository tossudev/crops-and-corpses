using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class PlayerSpriteController : Skeleton2D
{
	public AnimationPlayer animPlayer;

	float scaleForwards = 0.15f;
	float scaleBackwards = -0.15f;
	float animSpeedMultiplier = 200;
	public bool usingTool;
	public bool isFlipped;
	public bool usingRanged;

	[Export] bool isNpc;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		usingTool = false;
		isFlipped = false;
	}

	public void UpdateSprite(Vector2 movement)
	{
		if (movement.X != 0.0 || movement.Y != 0.0)
		{
			animPlayer.SpeedScale = movement.Length() / animSpeedMultiplier;

			if (!usingTool && !usingRanged)
				animPlayer.Play("walk");
			else
				animPlayer.Play("legs");

			// if still or player mouse is to the left of the player
			if (movement.X >= 1.0 && isFlipped)
			{
				Scale = new Vector2(scaleForwards, scaleForwards);
				isFlipped = false;
			}

			else if (movement.X <= -1.0 && !isFlipped)
			{
				Scale = new Vector2(scaleBackwards, scaleForwards);
				isFlipped = true;
			}
		}
		else
		{
			if (!usingTool && !usingRanged)
				animPlayer.Play("idle");
			else
				animPlayer.Play("idle_legs");

		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (usingRanged)
		{
			var torso = GetBoneNode(PlayerBone.Torso);

			// angle from player torso to mouse
			float angle = GlobalPosition.DirectionTo(GetGlobalMousePosition()).Angle() - torso.GlobalRotation;
			angle *= 180 / Mathf.Pi;

			const float LEFT_ARM_OFFSET = -45 - 15;
			const float RIGHT_ARM_OFFSET = -90 - 15;

			if (!isFlipped)
			{
				RotateBone(PlayerBone.Left_Arm, angle + LEFT_ARM_OFFSET);
				RotateBone(PlayerBone.Right_Arm, angle + RIGHT_ARM_OFFSET);
			}
			else
			{
				RotateBone(PlayerBone.Left_Arm, -angle + LEFT_ARM_OFFSET);
				RotateBone(PlayerBone.Right_Arm, -angle + RIGHT_ARM_OFFSET);
			}

			return;
		}

		if (!usingTool) return;

		if (!isFlipped && GetGlobalMousePosition().X < GlobalPosition.X)
		{
			Scale = new Vector2(scaleBackwards, scaleForwards);
			isFlipped = true;
		}
		else if (isFlipped && GetGlobalMousePosition().X > GlobalPosition.X)
		{
			Scale = new Vector2(scaleForwards, scaleForwards);
			isFlipped = false;
		}
	}



	public void FlipBoneX(PlayerBone bone)
	{
		var boneNode = GetBoneNode(bone);

		boneNode.Scale *= new Vector2(-1, 1);
	}

	public void FlipBoneY(PlayerBone bone)
	{
		var boneNode = GetBoneNode(bone);

		boneNode.Scale *= new Vector2(1, -1);
	}

	public void RotateBone(PlayerBone bone, float degrees)
	{
		var boneNode = GetBoneNode(bone);

		boneNode.RotationDegrees = degrees;
	}

	private Bone2D GetBoneNode(PlayerBone bone)
	{
		var torso = GetNode<Bone2D>("TorsoBone");
		switch (bone)
		{
			case PlayerBone.Head:
				return torso.GetNode<Bone2D>("HeadBone");

			case PlayerBone.Left_Leg:
				return torso.GetNode<Bone2D>("LegLBone");

			case PlayerBone.Right_Leg:
				return torso.GetNode<Bone2D>("LegRBone");

			case PlayerBone.Left_Arm:
				return torso.GetNode<Bone2D>("ArmLBone");

			case PlayerBone.Right_Arm:
				return torso.GetNode<Bone2D>("ArmRBone");

			case PlayerBone.Torso:
				return torso;

			default:
				return null;
		}
	}
}

public enum PlayerBone
{
	Head,
	Left_Leg,
	Right_Leg,
	Left_Arm,
	Right_Arm,
	Torso,
}
