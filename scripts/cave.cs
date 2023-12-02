using Godot;
using System;

public partial class cave : Node2D
{
	[Export] private Node2D _bridge;
	[Export] private Node2D _stalagmite;

	private bool _bridgeOpen = false;

	public override async void _Ready()
	{
		if (await SceneInfo.GetCaveBridgeOpen())
		{
			OpenBridge();
		}
	}

	private void OpenBridge()
	{
		if (_bridge != null)
			_bridge.Visible = true;
		_stalagmite.QueueFree();
	}
}
