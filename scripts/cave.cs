using Godot;
using System;

public partial class cave : Node2D
{
	[Export] private Node2D _bridge;
	[Export] private Node2D _stalagmite;
	[Export] private AnimationPlayer _sceneTransition;

	public override void _Ready()
	{
		PlayerController player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
		_sceneTransition = player.GetNode<CanvasLayer>("%PlayerOverlay").GetNode<AnimationPlayer>("%SceneTransition");
		_sceneTransition.SpeedScale = 0.75f;
		_sceneTransition?.Play("fade_in");

		if (SaveData.townHallStats.isCaveStalagmiteMined)
		{
			OpenBridge();
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
		_stalagmite.QueueFree();
	}
}
