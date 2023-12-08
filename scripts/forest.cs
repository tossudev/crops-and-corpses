using Godot;
using System;

public partial class forest : Node2D
{
	[Export] private Node2D _bridge;
	[Export] private Node2D _fallingTree;
	[Export] private Node2D _bridgeQuest;
	[Export] private Node2D _ruinsBridge;
	[Export] private AnimationPlayer _sceneTransition;

	public override void _Ready()
	{
		_sceneTransition = GetNodeOrNull<AnimationPlayer>("%SceneTransition");
		_sceneTransition.SpeedScale = 0.75f;
		_sceneTransition?.Play("fade_in");

		PlayerController player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
		player.GetNode<CanvasLayer>("PlayerOverlay").Visible = true;

		if (SaveData.townHallStats.isDIYBridgeBuilt)
		{
			OpenBridge();
		}

		if (SaveData.townHallStats.isRuinsUnlocked)
		{
			OpenRuinsBridge();
		}
	}

	private PlayerController GetNodeInGroup(string v)
	{
		throw new NotImplementedException();
	}

	private void OpenBridge()
	{
		if (_bridge != null)
			_bridge.Visible = true;
		_fallingTree.QueueFree();
	}

	private void OpenRuinsBridge()
	{
		if (_ruinsBridge != null)
			_ruinsBridge.Visible = true;
		_bridgeQuest.QueueFree();
	}
}
