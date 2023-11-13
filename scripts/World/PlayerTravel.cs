using Godot;
using System;

public partial class PlayerTravel : Node
{
	void TravelTown(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && button.IsPressed() && button.ButtonIndex == MouseButton.Left)
		{
			GetTree().ChangeSceneToFile("res://scenes/town.tscn");
		}
	}
	void TravelRiverside(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && button.IsPressed() && button.ButtonIndex == MouseButton.Left)
		{
			GetTree().ChangeSceneToFile("res://scenes/riverside.tscn");
		}
	}

	void TravelRuins(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && button.IsPressed() && button.ButtonIndex == MouseButton.Left)
		{
			GetTree().ChangeSceneToFile("res://scenes/ruins.tscn");
		}
	}

	void TravelForest(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && button.IsPressed()&& button.ButtonIndex == MouseButton.Left)
		{
			GetTree().ChangeSceneToFile("res://scenes/forest.tscn");
		}
	}
}
