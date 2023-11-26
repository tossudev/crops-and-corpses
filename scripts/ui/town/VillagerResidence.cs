using Godot;
using System;
using Godot.Collections;

public partial class VillagerResidence : Control
{
	
	//TODO: Get villagers to enter and execute appropriate functions
	
	public const string VILLAGER_FACE_BUTTON_FILEPATH = "res://scenes/ui/town_ui/villager_face_button.tscn";
	
	GridContainer _villagerFaceButtonParentGrid;
	const string VILLAGER_GRID_NODENAME = "%VillagerParentGrid";
	
	public override void _Ready()
	{
		_villagerFaceButtonParentGrid = GetNode<GridContainer>(VILLAGER_GRID_NODENAME);
    }
	
	Array<int> _currentResidents = new ();
	public void VillagerEnterBuilding(Villager newVillager)
	{
		if (newVillager == null)
		{
			GD.PushError("Villager trying to enter building was null");
			return;
		}
			
		newVillager.EnterShelter();
		_currentResidents.Add(newVillager.rawData.id);
		
		VillagerFaceButton villagerFaceButton = 
			(VillagerFaceButton) GD.Load<PackedScene>(VILLAGER_FACE_BUTTON_FILEPATH).Instantiate<Control>();
					
		villagerFaceButton.InitButton(newVillager);
		
		_villagerFaceButtonParentGrid.AddChild(villagerFaceButton);
    }

	public void OpenVillagerDialoguePanel(Villager villager)
	{
		DialogueControl.instance.OpenDialogueWindow(villager);
	}

	public void CloseDialoguePanel()
	{
		DialogueControl.instance.ExitDialogue();
	}
	
	public void VillagerExitBuilding (Villager leavingVillager)
	{
		int villagerInstanceId = leavingVillager.rawData.id;
		
		if (_currentResidents.Contains(villagerInstanceId))
		{
			_currentResidents.Remove(villagerInstanceId);

			foreach (var node in _villagerFaceButtonParentGrid.GetChildren())
			{
				if (node is not VillagerFaceButton villagerFaceButton) continue;
				if (villagerFaceButton.id != villagerInstanceId) continue;
				
				villagerFaceButton.QueueFree();
				leavingVillager.ExitShelter();
				break;
			}
		}
		else
		{
			GD.PushError("Leaving villager was not in currentResidents Array");
		}
	}
	
}
