using Godot;
using System;

public partial class ruins : Node2D
{
	[Export] private Node2D _caveBlockage;

	public override async void _Ready()
	{
		if (await SceneInfo.GetRuinsCaveOpen())
		{
			OpenCave();
		}
	}

	private void OpenCave()
	{
		_caveBlockage.QueueFree();
	}
}
