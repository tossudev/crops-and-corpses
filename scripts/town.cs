using Godot;
using System;

public partial class town : Node2D
{
	[Export] private AnimationPlayer _sceneTransition;

	public override void _Ready()
	{
		PlayerController player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
		_sceneTransition = player.GetNode<CanvasLayer>("%PlayerOverlay").GetNode<AnimationPlayer>("%SceneTransition");
		_sceneTransition.SpeedScale = 0.75f;
		_sceneTransition?.Play("fade_in");
	}

	private PlayerController GetNodeInGroup(string v)
	{
		throw new NotImplementedException();
	}
}
