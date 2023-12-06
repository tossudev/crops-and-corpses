using Godot;
using System;
using Godot.Collections;

public partial class VillagerResidence : Control
{
	public const string VILLAGER_FACE_BUTTON_FILEPATH = "res://scenes/ui/town_ui/villager_face_button.tscn";
	
	GridContainer _villagerFaceButtonParentGrid;
	const string VILLAGER_GRID_NODENAME = "%VillagerParentGrid";

	public int id { get; private set; }
	public bool isBroken { get; private set; }

	
	[Export] int _housingCapacity;
	public int housingCapacity => _housingCapacity;
	
	Array<VillagerRawData> _allResidents = new ();
	Array<int> _currentResidentIds = new ();

	[Export] bool _isTownHall;
	public bool hasRoomForMoreVillagers => _isTownHall || _allResidents.Count < _housingCapacity;
	
	
	public override void _Ready()
	{
		if (_isTownHall) return;
		
		// TODO:
		_villagerFaceButtonParentGrid = GetNodeOrNull<GridContainer>(VILLAGER_GRID_NODENAME);
		
		RegisterResidence();
	}

	public void SetFaceButtonParentGrid(GridContainer container)
	{
		_villagerFaceButtonParentGrid = container;
	}
	
	async void RegisterResidence()
	{
		await TaskExtensions.SuspendWhile(() =>
			VillagerManager.villagerManagerInstance == null || !SaveData.firstLoadComplete, GD.Randi() % 2000 + 100);
		
		id = _isTownHall
			? 0
			: VillagerManager.villagerManagerInstance.allVillagerResidences.Count + 1;
		
		VillagerManager.villagerManagerInstance.AddNewResidence(this);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (VillagerManager.villagerManagerInstance?.allVillagerResidences.Contains(this) ?? false)
		{
			VillagerManager.villagerManagerInstance.RemoveResidence(this);
		}
	}
	
	public async void VillagerEnterBuilding(Villager newVillager)
	{
		
		if (newVillager == null)
		{
			GD.PushError("Villager trying to enter building was null");
			return;
		}
			
		newVillager.EnterShelter();
		_currentResidentIds.Add(newVillager.rawData.id);
		
		VillagerFaceButton villagerFaceButton = 
			(VillagerFaceButton) GD.Load<PackedScene>(VILLAGER_FACE_BUTTON_FILEPATH).Instantiate<Control>();
					
		villagerFaceButton.InitButton(newVillager);

		if (_isTownHall)
		{
			await TaskExtensions.SuspendWhile(() =>
				TownHallMenu.menuInstance == null || TownHallMenu.menuInstance.villagerResidence == null, 100);
		}
		
		_villagerFaceButtonParentGrid.AddChild(villagerFaceButton);
    }
	
	public void VillagerExitBuilding (Villager leavingVillager)
	{
		int villagerInstanceId = leavingVillager.rawData.id;
		
		if (_currentResidentIds.Contains(villagerInstanceId))
		{
			_currentResidentIds.Remove(villagerInstanceId);

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

	/// <summary>
	/// Adds a resident to 
	/// </summary>
	/// <param name="villager"></param>
	/// <returns></returns>
	public int AddResident(VillagerRawData data)
	{
		_allResidents.Add(data);
		return id;
	}
    
	public void OnBreak()
	{
		isBroken = true;
		ReassignAllResidents();
		_allResidents.Clear();
	}
	
	public void OnFixed()
	{
		isBroken = false;
	}

	void ReassignAllResidents()
	{
		foreach (var villagerRawData in _allResidents)
		{
			villagerRawData.TrySetHome();
		}
	}
}
