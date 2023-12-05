using Godot;
using System;

public partial class ruins : Node2D
{
	[Export] private Node2D _caveBlockage;

	public override void _Ready()
	{
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
