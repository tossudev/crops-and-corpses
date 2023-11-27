using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Portal : Node2D
{
	[Export] private SceneName _targetScene;

	public override void _PhysicsProcess(double delta)
	{

	}

	public void OnBodyEntered(Node body)
	{
		if (body is PlayerController)
		{
			var player = body as PlayerController;
			switch (_targetScene)
			{
				case SceneName.Town:
					SceneManager.ChangeScene(this, Scene.Town);
					break;

				case SceneName.Cave:
					SceneManager.ChangeScene(this, Scene.Cave);
					break;

				case SceneName.Forest:
					SceneManager.ChangeScene(this, Scene.Forest);
					break;

				case SceneName.Ruins:
					SceneManager.ChangeScene(this, Scene.Ruins);
					break;

				default:
					GD.PrintErr("Unknown scene name: " + _targetScene);
					break;
			}
		}
	}
}

enum SceneName
{
	Town,
	Cave,
	Forest,
	Ruins
}
