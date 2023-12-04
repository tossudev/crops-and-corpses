using Godot;
using System;

public partial class forest : Node2D
{
	[Export] private Node2D _bridge;
	[Export] private Node2D _fallingTree;
	[Export] private Node2D _bridgeQuest;
	[Export] private Node2D _ruinsBridge;

	public override async void _Ready()
	{
		if (await SceneInfo.GetForestBridgeOpen())
		{
			OpenBridge();
		}

		if (await SceneInfo.GetForestBuildABridgeOpen())
		{
			OpenRuinsBridge();
		}
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
