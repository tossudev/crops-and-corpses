using Godot;
using Godot.Collections;
using System;
using System.Runtime.ExceptionServices;

public partial class PortalManager : Node2D
{
	[Export] private SceneID _sceneID;
	[Export] private PlayerController _player;

	public override void _Ready()
	{
		foreach (var child in GetChildren())
		{
			if (child is Portal)
			{
				if (((Portal)child).targetSceneID == PlayerInfo.sceneID)
				{
					_player.Position = ((Portal)child).exitPosition.GlobalPosition;
				}
			}
		}

		PlayerInfo.sceneID = _sceneID;
	}

	public void PortalTo(SceneID sceneID)
	{
		switch (sceneID)
		{
			case SceneID.Town:
				SceneManager.ChangeScene(this, Scene.Town);
				break;

			case SceneID.Cave:
				SceneManager.ChangeScene(this, Scene.Cave);
				break;

			case SceneID.Forest:
				SceneManager.ChangeScene(this, Scene.Forest);
				break;

			case SceneID.Ruins:
				SceneManager.ChangeScene(this, Scene.Ruins);
				break;

			default:
				GD.PrintErr("Unknown scene name: " + sceneID);
				break;
		}
	}
}

public enum SceneID
{
	None,
	Cave,
	Forest,
	Ruins,
	Town,
}
