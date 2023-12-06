using Godot;
using System;

public partial class TownHallControl : Node2D
{
	void OnTownHallInput(Node viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is not InputEventMouseButton { Pressed: true } mouseEvent) return;
		
		if (mouseEvent.ButtonIndex == MouseButton.Left)
		{
			TownHallMenu.menuInstance.OpenMainPanel();
		}
	}


	const string VILLAGER_RESIDENCE_NODENAME = "%VillagerResidenceComponent";
	public override void _Ready()
	{
		base._Ready();

		TownManager.SetTownHallPosition(GlobalPosition - new Vector2(-10, -10));
		
		var residence = GetNode<VillagerResidence>(VILLAGER_RESIDENCE_NODENAME);
		TownHallMenu.menuInstance.villagerResidence = residence;
		
		residence.SetFaceButtonParentGrid(TownHallMenu.menuInstance.GetNodeOrNull<GridContainer>("%VillagerParentGrid"));
	}
}
