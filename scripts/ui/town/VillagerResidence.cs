using Godot;
using System;
using Godot.Collections;

public partial class VillagerResidence : Control
{
	
	//TODO: Get villagers to enter and execute appropriate functions
	
	const string VILLAGER_FACE_BUTTON_FILEPATH = "res://scenes/ui/town_ui/villager_face_button.tscn";
	
	GridContainer _villagerFaceButtonParentGrid;
	const string VILLAGER_GRID_NODENAME = "%VillagerParentGrid";
	
	public override void _Ready()
	{
		_villagerFaceButtonParentGrid = GetNode<GridContainer>(VILLAGER_GRID_NODENAME);
	}

	Array<ulong> _currentResidents;
	public void VillagerEnterBuilding(Villager_Info newVillager)
	{
		// TODO: Not sure if VillagerInfo is the correct class to pass as argument
		// (To create a new VillagerFaceButton) 
		
		_currentResidents.Add(newVillager.GetInstanceId());
		
		VillagerFaceButton villagerFaceButton = 
			(VillagerFaceButton) GD.Load<PackedScene>(VILLAGER_FACE_BUTTON_FILEPATH).Instantiate<Control>();
					
		villagerFaceButton.InitButton(newVillager);
		
		_villagerFaceButtonParentGrid.AddChild(villagerFaceButton);
    }

	public void VillagerExitBuilding (Villager_Info leavingVillager)
	{
		ulong villagerInstanceId = leavingVillager.GetInstanceId();
		
		if (_currentResidents.Contains(villagerInstanceId))
		{
			_currentResidents.Remove(villagerInstanceId);

			foreach (var node in _villagerFaceButtonParentGrid.GetChildren())
			{
				if (node is not VillagerFaceButton villagerFaceButton) continue;
				if (villagerFaceButton.id != villagerInstanceId) continue;
				
				villagerFaceButton.QueueFree();
				break;
			}
		}
		else
		{
			GD.PushError("Leaving villager was not in currentResidents Array");
		}
	}
	
}
