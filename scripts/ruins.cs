using Godot;
using System;

public partial class ruins : Node2D
{
	[Export] private Node2D _caveBlockage;
	[Export] private AnimationPlayer _sceneTransition;

	public override void _Ready()
	{
		PlayerController player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
		_sceneTransition = player.GetNode<CanvasLayer>("%PlayerOverlay").GetNode<AnimationPlayer>("%SceneTransition");
		_sceneTransition.SpeedScale = 0.75f;
		_sceneTransition?.Play("fade_in");

		if (SaveData.townHallStats.isMineshaftUnlocked)
		{
			OpenCave();
		}
	}

	private void OpenCave()
	{
		_caveBlockage.QueueFree();
	}
}
