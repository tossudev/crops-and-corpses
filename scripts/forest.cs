using Godot;
using System;

public partial class forest : Node2D
{
	[Export] private Node2D _bridge;
	[Export] private Node2D _fallingTree;

	public override async void _Ready()
	{
		if (await SceneInfo.GetForestBridgeOpen())
		{
			OpenBridge();
		}
	}

	private void OpenBridge()
	{
		if (_bridge != null)
			_bridge.Visible = true;
		_fallingTree.QueueFree();
	}
}
