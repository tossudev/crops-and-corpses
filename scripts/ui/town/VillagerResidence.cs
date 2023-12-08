using Godot;
using System;
using Godot.Collections;

public partial class VillagerResidence : Control
{
	public const string VILLAGER_FACE_BUTTON_FILEPATH = "res://scenes/ui/town_ui/villager_face_button.tscn";
	
	GridContainer _villagerFaceButtonParentGrid;
	const string VILLAGER_GRID_NODENAME = "%VillagerParentGrid";
	
	Button _closeButton;
	const string CLOSE_BUTTON_CONTAINER_NODENAME = "%CloseButtonContainer";
	const string CLOSE_BUTTON_NODENAME = "%CloseButton";
    
	Button _demolishButton;
	const string DEMOLISH_BUTTON_NODENAME = "%DemolishButton";
	
	BuildingDemolishMenu _demolishMenu;
	const string DEMOLISH_MENU_NODENAME = "%BuildingDemolishMenu";

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
		ClosePanel();
		if (_isTownHall) return;

		_closeButton = GetNode<MarginContainer>(CLOSE_BUTTON_CONTAINER_NODENAME).GetNode<Button>(CLOSE_BUTTON_NODENAME);
		_closeButton.Pressed += ClosePanel;

		_demolishMenu = Owner.GetNode<BuildingDemolishMenu>(DEMOLISH_MENU_NODENAME);
		
		_demolishButton = GetNode<Button>(DEMOLISH_BUTTON_NODENAME);
		_demolishButton.Pressed += () =>
		{
			ClosePanel();
			_demolishMenu.OpenMainPanel();
		};

		_villagerFaceButtonParentGrid = GetNodeOrNull<GridContainer>(VILLAGER_GRID_NODENAME);

		
		RegisterResidence();
	}

	public void SetFaceButtonParentGrid(GridContainer container)
	{
		_villagerFaceButtonParentGrid = container;
	}

	void OnBuildingInput(Node viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is not InputEventMouseButton { Pressed: true } mouseEvent) return;

		if (mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if(PlayerInventoryController.heldItem == null || PlayerInventoryController.heldItem.id != 370)
			{
                OpenPanel();
            }
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		if (@event.IsActionPressed("close_townhall_menu"))
		{
			ClosePanel();
		}
	}
	
	void OpenPanel()
	{
		Visible = true;
	}

	void ClosePanel()
	{
		Visible = false;
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
		int villagerId = leavingVillager.rawData.id;
		
		if (_currentResidentIds.Contains(villagerId))
		{
			_currentResidentIds.Remove(villagerId);

			foreach (var node in _villagerFaceButtonParentGrid.GetChildren())
			{
				if (node is not VillagerFaceButton villagerFaceButton) continue;
				if (villagerFaceButton.id != villagerId) continue;
				
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
