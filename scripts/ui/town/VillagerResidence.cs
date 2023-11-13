using Godot;
using System;
using Godot.Collections;

public partial class VillagerResidence : Control
{
	
	//TODO: Get villagers to enter and execute appropriate functions
	
	const string VILLAGER_FACE_BUTTON_FILEPATH = "res://scenes/ui/town_ui/villager_face_button.tscn";
	
	GridContainer _villagerFaceButtonParentGrid;
	const string VILLAGER_GRID_NODENAME = "%VillagerParentGrid";

	DialogueControl _villagerDialoguePanel;
	const string VILLAGER_DIALOGUE_PANEL_NODENAME = "%VillagerDialoguePanel";
	
	public override void _Ready()
	{
		_villagerFaceButtonParentGrid = GetNode<GridContainer>(VILLAGER_GRID_NODENAME);
		_villagerDialoguePanel = GetNode<DialogueControl>(VILLAGER_DIALOGUE_PANEL_NODENAME);
		
		CloseDialoguePanel();
	}
	
	Array<ulong> _currentResidents = new ();
	public void VillagerEnterBuilding(Villager newVillager)
	{
		if (newVillager == null)
		{
			GD.PushError("Villager trying to enter building was null");
			return;
		}
			
		newVillager.EnterShelter();
		_currentResidents.Add(newVillager.GetInstanceId());
		
		VillagerFaceButton villagerFaceButton = 
			(VillagerFaceButton) GD.Load<PackedScene>(VILLAGER_FACE_BUTTON_FILEPATH).Instantiate<Control>();
					
		villagerFaceButton.InitButton(newVillager);
		
		_villagerFaceButtonParentGrid.AddChild(villagerFaceButton);
    }

	public void OpenVillagerDialoguePanel(Villager villager)
	{
		_villagerDialoguePanel.AssignVillager(villager);
		_villagerDialoguePanel.OpenDialogueWindow();
	}

	public void CloseDialoguePanel()
	{
		_villagerDialoguePanel.ExitDialogue();
	}
	
	public void VillagerExitBuilding (Villager leavingVillager)
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
