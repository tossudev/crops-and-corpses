using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.NativeInterop;

public partial class VillagerOccupationTab : ScrollContainer
{
	GridContainer _villagerGrid;
	const string VILLAGER_GRID_NODENAME = "%GridContainer";
    
	[Export] VillagerOccupation _occupation;


	List<Villager> _occupationList = new ();
	readonly List<VillagerFaceButton> _currentFaceButtons = new ();

	int _occupationCountLastTime;
	bool _initialized;
	bool _initStarted;
	async void Initialize ()
	{
		if (_initialized || _initStarted) return;

		_initStarted = true;
		await TaskExtensions.SuspendWhile(() => VillagerManager.villagerManagerInstance == null, 120); 
		
		_villagerGrid = GetNode<GridContainer>(VILLAGER_GRID_NODENAME);

		foreach (var child in _villagerGrid.GetChildren())
		{
			child.QueueFree();
		}
		
		_occupationList = _occupation switch
		{
			VillagerOccupation.Builder => VillagerManager.villagerManagerInstance.BuilderVillagers,
			VillagerOccupation.Farmer => VillagerManager.villagerManagerInstance.farmerVillagers,
			VillagerOccupation.Soldier => VillagerManager.villagerManagerInstance.soldierVillagers,
			VillagerOccupation.Woodcutter => VillagerManager.villagerManagerInstance.woodcutterVillagers,
			VillagerOccupation.Miner => VillagerManager.villagerManagerInstance.minerVillagers,
			_ => null
		};
        
		_initialized = true;
		UpdateVillagerFaceButtons();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (!Visible) return;

		if (TownManager.EveryXSecond(1))
		{
			if (!_initialized)
			{
				Initialize();
				return;
			}

			if (_occupationList.Count != _occupationCountLastTime)
			{
				UpdateVillagerFaceButtons();
			}

			_occupationCountLastTime = _occupationList.Count;
		}
	}

	void UpdateVillagerFaceButtons()
	{
		if (!_initialized) Initialize();

		List<int> occupationListIds = new();
		
		foreach (var villager in _occupationList)
		{
			int villagerId = villager.rawData.id;
			
			occupationListIds.Add(villagerId);
			
			if (_currentFaceButtons.Any(face => face.id == villagerId)) continue;
			
			CreateVillagerButton(villager);
		}


		for (var index = 0; index < _currentFaceButtons.Count; index++)
		{
			var button = _currentFaceButtons[index];
			if (occupationListIds.Contains(button.id)) continue;
			
			button.QueueFree();
			_currentFaceButtons.RemoveAt(index);
			index--;
		}
	}

    void CreateVillagerButton(Villager newVillager)
	{
		if (newVillager == null)
		{
			GD.PushError("Villager was null @VillagerOccupationTab");
			return;
		}
        
		VillagerFaceButton villagerFaceButton = 
			(VillagerFaceButton) GD.
				Load<PackedScene>(VillagerResidence.VILLAGER_FACE_BUTTON_FILEPATH).Instantiate<Control>();
					
		villagerFaceButton.InitButton(newVillager);
		
		_currentFaceButtons.Add(villagerFaceButton);
		_villagerGrid.AddChild(villagerFaceButton);
	}

	public void OpenVillagerDialoguePanel(Villager villager)
	{
		// TODO
	}

	public void CloseDialoguePanel()
	{
		// TODO
	}
}
