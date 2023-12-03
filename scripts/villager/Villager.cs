using Godot;
using System;
using System.Collections.Generic;


public enum VillagerOccupation
{
	Builder,
	Farmer,
	Soldier,
	Woodcutter,
	Miner
}

public partial class Villager : CharacterBody2D
{
	//[Export] VillagerManager _villagerManager;
	Vector2 _targetPosition;
	Timer _taskTimer;
	Timer _chooseTaskTimer;
	[Export] NavigationAgent2D navMeshAgent;
	//[Export] NavigationRegion2D navRegionArea;
	//List<Node2D> _fenceList = new List<Node2D>();
	Plant _currentPlant;
	ArcherTower _archerTower;
	BuildingHealth _fenceHealth;
	Node2D _buildings;
	Node2D _fences;
	BuildingHealth _currentBuilding;
	Sprite2D _villagerSprite;
	int _plantIndex = 0;
	int _archerTowerIndex = 0;
	int _fenceIndex = 0;
	float _speed = 0;
	bool _taskStarted = false;
	int _resourceTaskCounter = 0;
	const string PLAYER_NODENAME = "%Player";
	const string STREETSIGN_NODENAME = "%StreetSign";
	CharacterBody2D _player;
	public bool needRescue = false;

	[Export] VillagerInfo _info;
	public VillagerInfo villagerInfo => _info;

	VillagerRawData _rawData;
	public VillagerRawData rawData => _rawData;

	bool _inTownScene;

	public List<Villager> currentOccupationList;

	public VillagerResidence currentResidence;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetParent().GetNodeOrNull<CharacterBody2D>(PLAYER_NODENAME);
		_villagerSprite = GetNode<Sprite2D>("Sprite2D");
		_buildings = (Node2D)GetTree().GetFirstNodeInGroup("buildings");

		_taskTimer = new Timer
		{
			WaitTime = GD.RandRange(0.8f, 1.25f)
		};
		_taskTimer.Timeout += State;
		AddChild(_taskTimer);
		_taskTimer.Start();
	}

	public void InitializeVillager(VillagerRawData data)
	{
		_rawData = data;

		_info.InitializeVillagerInfo(data);

		// TODO: This needs to be changed once the rig sprites are in place and saved
		_villagerSprite.Texture = VillagerManager.villagerManagerInstance.GetNewVillagerData().texture;


		_inTownScene = SceneManager.IsCurrentScene(this, Scene.Town);

		if (!_inTownScene)
		{
			needRescue = true;
			SetCurrentState(VillagerState.RescueQuest);
		}
		else
		{
			needRescue = false;
		}
	}

	public void Teleport(Vector2 coordinates)
	{
		GlobalPosition = coordinates;
		SavePosition();
	}

	void SetCurrentState(VillagerState state)
	{
		rawData.currentState = state;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.DistanceTo(_targetPosition) > 5)
		{
			Movement(_targetPosition);
		}
		if (_taskStarted && _resourceTaskCounter == 5)
		{
			ResourceGatheringDone();
		}
	}

	public void EnterShelter()
	{
		SetCurrentState(VillagerState.InShelter);
		_targetPosition = GlobalPosition;
		Visible = false;
	}

	public void ExitShelter()
	{
		SetCurrentState(VillagerState.ChooseTask);
		Visible = true;
	}

	void State()
	{
		switch (rawData.currentState)
		{
			case VillagerState.RoamAround:
				RoamAround();
				break;

			case VillagerState.FollowPlayer:
				FollowPlayer();
				break;

			case VillagerState.ChooseTask:
				ChooseTask();
				break;

			case VillagerState.FarmingTask:
				CheckPlants();
				break;

			case VillagerState.FindWoodTask:
				GatherResources();
				break;

			case VillagerState.FindStoneTask:
				GatherResources();
				break;

			case VillagerState.RescueQuest:
				WaitingRescue();
				break;

			case VillagerState.FixFence:
				FixBuildings();
				break;

			case VillagerState.FindArcherTower:
				EnterArcherTower();
				break;

			case VillagerState.FindShelter:

				FindShelter();
				break;

			case VillagerState.InShelter:
				BeInShelter();
				break;

			case VillagerState.Homeless:
				FindShelter();
				break;
			
			default:
				GD.Print("State not found");
				break;
		}
		//GD.Print(_state);
	}

	public void _on_button_button_up()
	{
		QuestManager questManager = GetNode<QuestManager>("/root/QuestManager");
		if (!_inTownScene && needRescue)
		{
			OpenRescueDialogue();
			questManager.GetActiveQuest().ChangeQuestDescription("Take the villager to Street Sign");
		}
		else
		{
			OpenDialogue();
		}
	}

	public void OpenDialogue()
	{
		DialogueControl.instance.OpenDialogueWindow(this);
	}

	public void OpenRescueDialogue()
	{
		GD.Print("You saved me");
		// Tähä joku button tai joku ?????
		SetCurrentState(VillagerState.FollowPlayer);
	}

	const string VILLAGER_RESIDENCE_NODENAME = "%VillagerResidence";
	public void _on_area_2d_area_entered(Area2D area)
	{
		if (!area.Owner.HasNode(VILLAGER_RESIDENCE_NODENAME)) return;
		currentResidence = area.Owner.GetNodeOrNull<VillagerResidence>(VILLAGER_RESIDENCE_NODENAME);

		currentResidence?.VillagerEnterBuilding(this);
	}

	void Movement(Vector2 target)
	{
		_speed = 100;
		Vector2 _direction = (target - GlobalPosition).Normalized();
		Velocity = _direction * _speed;
		MoveAndSlide();
	}

	public void SavePosition()
	{
		rawData.SetCoordinates(GlobalPosition);
	}

	void RoamAround()
	{
		_targetPosition = GlobalPosition + CreateOffsetVector2(-200, 200);

		if (_chooseTaskTimer == null)
		{
			_chooseTaskTimer = new Timer
			{
				WaitTime = GD.RandRange(2, 5f),
				Autostart = true
			};

			_chooseTaskTimer.Timeout += ChooseTask;

			AddChild(_chooseTaskTimer);
		}
	}

	void FindShelter()
	{
		_targetPosition = rawData.homeId == 0
			? TownManager.townHallPosition - GlobalPosition + CreateOffsetVector2(-100, 100)
			: VillagerManager.villagerManagerInstance.FindResidenceById(rawData.homeId).GlobalPosition;
	}

	void BeInShelter()
	{
		if (rawData.currentState == VillagerState.Homeless) return;
		
		if (TimeManager.dayTime)
		{
			currentResidence.VillagerExitBuilding(this);
		}
	}

	Vector2 CreateOffsetVector2(double min, double max)
	{
		GD.Randomize();
		float x = (float)GD.RandRange(min, max);

		GD.Randomize();
		float y = (float)GD.RandRange(min, max);

		return new Vector2(x, y);
	}

	void ChooseTask()
	{
		if (_chooseTaskTimer != null)
		{
			_chooseTaskTimer.Paused = true;
			_chooseTaskTimer.QueueFree();
			_chooseTaskTimer = null;
		}

		VillagerState decision;

		switch (_rawData.currentOccupation)
		{
			case VillagerOccupation.Builder:
				decision = VillagerState.FixFence;
				break;

			case VillagerOccupation.Farmer:
				decision = VillagerState.FarmingTask;
				break;

			case VillagerOccupation.Soldier:
				if (TimeManager.dayTime)
				{
					if (!Visible)
					{
						ExitArcherTower();
					}
					decision = VillagerState.RoamAround;
				}
				else
				{
					if (FreeArcherTowerFound())
					{
						decision = VillagerState.FindArcherTower;
					}
					else
					{
						decision = VillagerState.FindShelter;
					}

				}
				break;

			case VillagerOccupation.Woodcutter:
				decision = VillagerState.FindWoodTask;
				break;

			case VillagerOccupation.Miner:
				decision = VillagerState.FindStoneTask;
				break;


			default:
				decision = VillagerState.RoamAround;
				break;
		}


		if (rawData.homeId == 0)
		{
			decision = VillagerState.Homeless;
		}

		// Night Time
		if (!TimeManager.dayTime && _inTownScene)
		{
			if (_rawData.currentOccupation != VillagerOccupation.Soldier)
			{
				decision = VillagerState.FindShelter;
			}
		}

		SetCurrentState(decision);
	}

	void GatherResources()
	{
		_targetPosition = TownManager.GetTownPlayerTravel(this).GlobalPosition;
		if (GlobalPosition.DistanceTo(_targetPosition) < 5)
		{
			_taskStarted = true;
			_villagerSprite.Visible = false;
			_resourceTaskCounter++;
		}
	}

	void ResourceGatheringDone()
	{
		Random rnd = new Random();
		_villagerSprite.Visible = true;
		if (_rawData.currentOccupation == VillagerOccupation.Miner)
		{
			int amount = rnd.Next(4, 21);
			GD.Print("Found: " + amount + " stone");
		}

		if (_rawData.currentOccupation == VillagerOccupation.Woodcutter)
		{
			int amount = rnd.Next(4, 21);
			GD.Print("Found: " + amount + " wood");
		}

		_taskStarted = false;
		_resourceTaskCounter = 0;
		ChooseTask();
	}

	void WaitingRescue()
	{
		_targetPosition = GlobalPosition + CreateOffsetVector2(-10, 10);
	}

	void CheckPlants()
	{
		var plants = FarmManager.instance.GetPlantedPlants();

		_currentPlant = plants.Count > 0
			? plants[_plantIndex]
			: null;

		if (_currentPlant == null)
		{
			SetCurrentState(VillagerState.RoamAround);
			return;
		}

		_currentPlant.isTendedTo = true;

		if (_currentPlant.GetGrowthState() == GrowthState.IsWilting || _currentPlant.GetGrowthState() == GrowthState.WaitWatering ||
			_currentPlant.GetGrowthState() == GrowthState.IsInfested)
		{
			_targetPosition = _currentPlant.GlobalPosition;
		}
		else
		{
			bool _allHarvestable = true;

			for (int i = 0; i < FarmManager.instance.GetPlantedPlants().Count; i++)
			{
				if (FarmManager.instance.GetPlantedPlants()[i].GetGrowthState() != GrowthState.IsHarvestable &&
					FarmManager.instance.GetPlantedPlants()[i].GetGrowthState() != GrowthState.IsDead)
				{
					_allHarvestable = false;
					break;
				}
			}
			if (_allHarvestable == false)
			{
				_plantIndex++;
				if (_plantIndex >= FarmManager.instance.GetPlantedPlants().Count)
				{
					_plantIndex = 0;
				}
				return;
			}

			SetCurrentState(VillagerState.RoamAround);
			return;
		}
		if (GlobalPosition.DistanceTo(_currentPlant.GlobalPosition) < 5)
		{
			GD.Print("I am at the plant yay");
			if (_currentPlant.GetGrowthState() == GrowthState.IsWilting || _currentPlant.GetGrowthState() == GrowthState.WaitWatering)
			{
				_currentPlant.WaterPlant();
			}
			if (_currentPlant.GetGrowthState() == GrowthState.IsInfested)
			{
				_currentPlant.CurePlant();
			}
			if (_currentPlant.GetGrowthState() != GrowthState.IsWilting && _plantIndex < FarmManager.instance.GetPlantedPlants().Count ||
			 _currentPlant.GetGrowthState() != GrowthState.WaitWatering && _plantIndex < FarmManager.instance.GetPlantedPlants().Count ||
			_currentPlant.GetGrowthState() != GrowthState.IsInfested && _plantIndex < FarmManager.instance.GetPlantedPlants().Count)
			{
				_plantIndex++;
			}
			if (_plantIndex == FarmManager.instance.GetPlantedPlants().Count)
			{
				_plantIndex = 0;
			}
		}
	}

	void FollowPlayer()
	{
		_speed = 200;
		_targetPosition = _player.GlobalPosition;
	}
	void FixBuildings()
	{
		if (_currentBuilding != null)
		{
			if (!_currentBuilding.isDamaged)
			{
				_currentBuilding = null;
			}
			else
			{
				_targetPosition = _currentBuilding.GlobalPosition;
				if (GlobalPosition.DistanceTo(_currentBuilding.GlobalPosition) < 200)
				{
					_currentBuilding.FixBrokenBuilding();
				}
			}
		}
		else
		{
			var archerTowers = VillagerManager.villagerManagerInstance.GetBrokenBuildingsOfType(BuildingType.ArcherTower);
			var fences = VillagerManager.villagerManagerInstance.GetBrokenBuildingsOfType(BuildingType.Fence);
			var houses = VillagerManager.villagerManagerInstance.GetBrokenBuildingsOfType(BuildingType.House);

			if (archerTowers.Count > 0)
			{
				_currentBuilding = archerTowers[(int)GD.Randi() % archerTowers.Count];
			}
			else if (fences.Count > 0)
			{
				_currentBuilding = fences[(int)GD.Randi() % fences.Count];
			}
			else if (houses.Count > 0)
			{
				_currentBuilding = houses[(int)GD.Randi() % houses.Count];
			}
			else
			{
				SetCurrentState(VillagerState.RoamAround);
			}
		}
	}

	bool FreeArcherTowerFound()
	{
		List<ArcherTower> _freeArcherTowers = VillagerManager.villagerManagerInstance.GetArcherTowerList();

		for (int i = 0; i < _freeArcherTowers.Count; i++)
		{
			_archerTower = VillagerManager.villagerManagerInstance.GetArcherTowerList()[_archerTowerIndex];

			if (!_archerTower.isOccupied)
			{
				return true;
			}
			_archerTowerIndex++;

			if (_archerTowerIndex == _freeArcherTowers.Count)
			{
				_archerTowerIndex = 0;
			}
		}
		return false;
	}

	void EnterArcherTower()
	{
		if (FreeArcherTowerFound())
		{
			_archerTower = VillagerManager.villagerManagerInstance.GetArcherTowerList()[_archerTowerIndex];
			_targetPosition = _archerTower.GlobalPosition;

			if (GlobalPosition.DistanceTo(_archerTower.GlobalPosition) < 200)
			{
				Visible = false;
				_archerTower.ActivateTower();
			}
		}
		else
		{
			SetCurrentState(VillagerState.RoamAround);
		}
	}

	void ExitArcherTower()
	{
		_archerTower.DeactivateTower();
		Visible = true;
	}
}


