using Godot;
using System;

public partial class PlayerTravel : Node2D
{
	const string RUINS_STREET_SIGN_GROUP = "%ruins_sign";
	Area2D ruinsSign;

	public override void _Ready()
	{
		base._Ready();
		ruinsSign = GetNode<Area2D>(RUINS_STREET_SIGN_GROUP);
		ruinsSign.Visible = true;
	
			
		
        if (!TownManager.currentTownStats.isRuinsUnlocked || SceneManager.IsCurrentScene(this, Scene.Ruins))
        {
            ruinsSign.Visible = false;
        }
        else
        {
            ruinsSign.Visible = true;
        }
    }


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
		if (@event is InputEventMouseButton button && button.IsPressed() && button.ButtonIndex == MouseButton.Left && TownManager.currentTownStats.isRuinsUnlocked)
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
