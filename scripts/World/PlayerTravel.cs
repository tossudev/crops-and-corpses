using Godot;
using System;

public partial class PlayerTravel : Node2D
{
	void TravelTown(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton button && button.IsPressed() && button.ButtonIndex == MouseButton.Left)
		{
			SceneManager.ChangeScene(this, Scene.Town);
		}
	}
	void TravelCave(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton button && button.IsPressed() && button.ButtonIndex == MouseButton.Left)
		{
			SceneManager.ChangeScene(this, Scene.Cave);
		}
	}

	void TravelRuins(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton button && button.IsPressed() && button.ButtonIndex == MouseButton.Left)
		{
			SceneManager.ChangeScene(this, Scene.Ruins);
		}
	}

	void TravelForest(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton button && button.IsPressed() && button.ButtonIndex == MouseButton.Left)
		{
			SceneManager.ChangeScene(this, Scene.Forest);
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		TownManager.townPlayerTravel = null;
	}
}
