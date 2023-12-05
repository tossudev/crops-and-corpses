using Godot;
using System;

public partial class cave : Node2D
{
	[Export] private Node2D _bridge;
	[Export] private Node2D _stalagmite;

	public override void _Ready()
	{
		if (SaveData.townHallStats.isCaveStalagmiteMined)
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
