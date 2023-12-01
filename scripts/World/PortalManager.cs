using Godot;
using Godot.Collections;
using System;
using System.Runtime.ExceptionServices;

public partial class PortalManager : Node2D
{
	[Export] private PortalID _sceneID;
	[Export] private PlayerController _player;

	public override void _Ready()
	{
		foreach (var child in GetChildren())
		{
			if (child is Portal)
			{
				GD.Print(((Portal)child).id);
				GD.Print(PlayerInfo.travelID);
				if (((Portal)child).id == PlayerInfo.travelID)
				{
					_player.Position = ((Portal)child).exitPosition.GlobalPosition;
				}
			}
		}

		PlayerInfo.travelID = _sceneID;
	}

	public void PortalTo(PortalID id)
	{
		_player.SaveState();
		switch (id)
		{
			case PortalID.Town:
				SceneManager.ChangeScene(this, Scene.Town);
				break;

			case PortalID.Cave:
				SceneManager.ChangeScene(this, Scene.Cave);
				break;

			case PortalID.Forest:
				SceneManager.ChangeScene(this, Scene.Forest);
				break;

			case PortalID.Ruins:
				SceneManager.ChangeScene(this, Scene.Ruins);
				break;

			default:
				GD.PrintErr("Unknown scene name: " + id);
				break;
		}
	}
}
