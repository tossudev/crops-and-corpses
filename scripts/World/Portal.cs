using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Portal : Node2D
{
	[Export] private Vector2 _targetPosition;
	[Export] private string _targetSceneName;

	public override void _Ready()
	{
	}

	public void OnBodyEntered(Node body)
	{
		if (body is PlayerController)
		{
			var player = body as PlayerController;
			SceneManager.ChangeScene(this, Scene.Town);
		}
	}
}
