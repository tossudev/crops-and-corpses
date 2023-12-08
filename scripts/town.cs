using Godot;
using System;

public partial class town : Node2D
{
	[Export] private AnimationPlayer _sceneTransition;

	public override void _Ready()
	{
		_sceneTransition = GetNodeOrNull<AnimationPlayer>("%SceneTransition");
		_sceneTransition.SpeedScale = 0.75f;
		_sceneTransition?.Play("fade_in");

		PlayerController player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
		player.GetNode<CanvasLayer>("PlayerOverlay").Visible = true;
	}

	private PlayerController GetNodeInGroup(string v)
	{
		throw new NotImplementedException();
	}
}
