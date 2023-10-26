using Godot;
using System;

public partial class PlayerTravel : Node
{
	void TravelHome(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && button.IsPressed())
		{
			GetTree().ChangeSceneToFile("res://scenes/town.tscn");
		}
	}
	void TravelSwamp(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && button.IsPressed())
		{
			GetTree().ChangeSceneToFile("res://scenes/world/street_sign_post.tscn");
		}
	}

	void TravelOakville(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && button.IsPressed())
		{
			GetTree().ChangeSceneToFile("res://scenes/world/street_sign_post.tscn");
		}
	}

	void TravelField(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && button.IsPressed())
		{
			GetTree().ChangeSceneToFile("res://scenes/world/street_sign_post.tscn");
		}
	}
}
