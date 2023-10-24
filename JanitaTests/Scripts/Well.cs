using Godot;
using System;

public partial class Well : Node2D
{
	bool _isPlayerNearby=false;

	void InteractWithWell(Node viewport, InputEvent @event, long shapeIdx)
	{
		if(@event is InputEventMouseButton button && _isPlayerNearby && button.IsPressed())
		{	
			GD.Print("Interacting with well");
			FarmManager.instance.FillWaterBucket();
		}
		
	}

	private void OnInteractable(Area2D body)
	{
		_isPlayerNearby=true;
	
	}

	private void OnNonInteractable(Area2D body)
	{
		_isPlayerNearby=false;
		
	}
}
