using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;


public enum VillagerOccupation
{
	Builder,
	Farmer,
	Soldier,
	Miner,
	Woodcutter,
}

public partial class Villager : CharacterBody2D
{
	//[Export] VillagerManager _villagerManager;
	PlayerSpriteController _spriteControl;
	Vector2 _targetPosition;
	Timer _taskTimer;
	Timer _chooseTaskTimer;
	[Export] NavigationAgent2D navMeshAgent;
	PackedScene villagerSkeleton;
	Plant _currentPlant;
	ArcherTower _archerTower;
	BuildingHealth _fenceHealth;
	Node2D _buildings;
	Node2D _fences;
	BuildingHealth _currentBuilding;
	Sprite2D _villagerSprite;
	AnimationPlayer _villagerAnimation;
	int _plantIndex = 0;
	float _speed = 100;
	bool _taskStarted = false;
	int _resourceTaskCounter = 0;
	const string PLAYER_NODENAME = "%Player";
	const string ANIMATION_PLAYER_NODENAME = "%VillagerAnimationPlayer";
	const string STREETSIGN_NODENAME = "%StreetSign";
	CharacterBody2D _player;
	public bool needRescue = false;

	CollisionShape2D _baseCollider;
	const string BASE_COLLIDER_NODENAME = "%BaseCollisionShape2D";

	
	const string VILLAGER_RESIDENCE_NODENAME = "%VillagerResidenceComponent";
	
	[Export] VillagerSkeleton _skeleton;
	public VillagerSkeleton skeleton => _skeleton;

	VillagerRawData _rawData;
	public VillagerRawData rawData => _rawData;

	bool _inTownScene;

	public List<Villager> currentOccupationList;

	public VillagerResidence currentResidence;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_villagerAnimation = skeleton.GetNodeOrNull<AnimationPlayer>(ANIMATION_PLAYER_NODENAME);
		_baseCollider = GetNodeOrNull<CollisionShape2D>(BASE_COLLIDER_NODENAME);

		_player = GetTree().GetFirstNodeInGroup("player") as CharacterBody2D;
		_buildings = (Node2D)GetTree().GetFirstNodeInGroup("buildings");
    }

	public void InitializeVillager(VillagerRawData data)
	{
		_rawData = data;

		skeleton.InitializeSkeleton(data);

		_inTownScene = SceneManager.IsCurrentScene(this, Scene.Town);

		VillagerManager.villagerManagerInstance.SetVillagerOccupation(this, data.currentOccupation);
		
		CheckIfNeedsRescue();
		
		_taskTimer = new Timer
		{
			WaitTime = GD.RandRange(0.8f, 1.25f)
		};
		_taskTimer.Timeout += State;
		AddChild(_taskTimer);
		_taskTimer.Start();
	}

	async void CheckIfNeedsRescue()
	{
		if (_inTownScene || _rawData.isTownPopulation) return;

		var quest = await PlayerInfo.GetActiveQuest();
		
		if (quest is not { type: QuestType.Rescue }) return;

		
		needRescue = true;
		SetCurrentState(VillagerState.RescueQuest);
	}
	
	public void Teleport(Vector2 coordinates)
	{
		GlobalPosition = coordinates;
		navMeshAgent.SetVelocityForced(Vector2.Zero);
		SavePosition();
	}

	void SetCurrentState(VillagerState state)
	{
		rawData.currentState = state;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_taskStarted && _resourceTaskCounter == 5)
		{
			ResourceGatheringDone();
		}

		Movement();
	}

	public void EnterShelter()
	{
		SetCurrentState(VillagerState.InShelter);
		
		StandStill();
		ToggleCollisionAndVisuals(false);
	}

	public void ExitShelter()
	{
		ChooseTask();
		ToggleCollisionAndVisuals(true);
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

			case VillagerState.FixBuildings:
				FixBuildings();
				break;

			case VillagerState.SoldierDuty:
				SoldierDuty();
				break;

			case VillagerState.FindShelter:

				FindShelter();
				break;

			case VillagerState.InShelter:
				BeInShelter();
				break;

			case VillagerState.Homeless:
				BeHomeless();
				break;
			
			default:
				GD.Print("State not found");
				break;
		}
    }

	public void _on_button_button_up()
	{
		if (!_inTownScene && needRescue)
		{
			OpenRescueDialogue();
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

	public async void OpenRescueDialogue()
	{
        var quest = await PlayerInfo.GetActiveQuest();

		if (!(quest?.stages.Contains(QuestStage.Rescue) ?? false)) return;
		
		if (quest.stages.Contains(QuestStage.Kill)) return;
		
		if (!quest.CompleteQuestStage(QuestStage.Rescue)) return;
		quest.ChangeQuestDescription("Take the villager to Street Sign");
		
		SetCurrentState(VillagerState.FollowPlayer);
	}

	public void _on_area_2d_area_entered(Area2D area)
	{
		currentResidence = area.Owner?.GetNodeOrNull<VillagerResidence>(VILLAGER_RESIDENCE_NODENAME);

		if (rawData.currentState == VillagerState.FindShelter)
		{
			currentResidence?.VillagerEnterBuilding(this);
		}
	}

	void ToggleCollisionAndVisuals(bool on)
	{
		Visible = on;
		_baseCollider.SetDeferred("disabled", !on);
	}

	public void SavePosition()
	{
		rawData.SetCoordinates(GlobalPosition);
	}

	void RoamAround()
	{
		SetNavMeshAgentPath(false,GlobalPosition + CreateOffsetVector2(-200, 200));

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

	void SetNavMeshAgentPath(bool isRunning, Vector2 direction)
	{
		_villagerAnimation.Play(isRunning ? "run" : "walk");

		float walkingFarmerMultiplier = rawData.currentOccupation == VillagerOccupation.Farmer
			? (float) TownManager.currentTownStats.farmerWalkSpeed / 2
			: 1;
		
		_speed = isRunning
			? 200
			: 100 * (walkingFarmerMultiplier);
		
		_targetPosition = direction;

		
		navMeshAgent.TargetPosition = _targetPosition;
		navMeshAgent.MaxSpeed = _speed;
	}

	void Movement()
	{
		if (navMeshAgent.IsNavigationFinished()) return;
			
		Vector2 nextPathPosition = navMeshAgent.GetNextPathPosition();
		
		Vector2 newVelocity = _speed * (nextPathPosition - GlobalPosition).Normalized();
        
		Velocity = newVelocity;
		
		MoveAndSlide();
		UpdateSprite();
	}
	
	void StandStill()
	{
		_villagerAnimation.Stop();
		_targetPosition = GlobalPosition;
	}
	
	void FindShelter()
	{
		Vector2 direction = rawData.homeId == 0
			? TownManager.townHallPosition
			: VillagerManager.villagerManagerInstance.FindResidenceById(rawData.homeId)?.GlobalPosition
			  ?? TownManager.townHallPosition;
		
		SetNavMeshAgentPath(true, direction);
		
		if (!TimeManager.dayTime) return;
		ChooseTask();
	}

	void BeInShelter()
	{
		if (currentResidence != null)
		{
			if (!TimeManager.dayTime) return;
			
			currentResidence.VillagerExitBuilding(this);
		}
		else
		{
			ChooseTask();
		}
	}

	void BeHomeless()
	{
		if (rawData.homeId == 0)
		{
			if (rawData.TrySetHome())
			{
				ChooseTask();
			}
			else
			{
				SetNavMeshAgentPath(false,GlobalPosition + CreateOffsetVector2(-150, 150));

			}
		}
		else
		{
			ChooseTask();
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

	void UpdateSprite()
	{
		Vector2 currentScale = skeleton.Scale;
		if (Velocity.X == 0.0 && Velocity.Y == 0.0) return;
		
		bool flip = false;
		
		if (Velocity.X > 0.5 && currentScale.X < 0)
		{
			flip = true;	
		}
		else if (Velocity.X < -0.5 && currentScale.X > 0)
		{
			flip = true;
		}
		
		
		if(flip)
		{
			skeleton.Scale = new Vector2(skeleton.Scale.X * -1, skeleton.Scale.Y);
		}
	}

	public void ChooseTask()
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
				decision = VillagerState.FixBuildings;
				break;

			case VillagerOccupation.Farmer:
				decision = VillagerState.FarmingTask;
				break;

			case VillagerOccupation.Soldier:
				decision = VillagerState.SoldierDuty;
				break;

			case VillagerOccupation.Woodcutter:
				decision = VillagerState.FindWoodTask;
				break;

			case VillagerOccupation.Miner:
				decision = VillagerState.FindStoneTask;
				break;
            
			default:
				decision = rawData.currentState == VillagerState.RescueQuest 
					? VillagerState.RescueQuest 
                    : VillagerState.RoamAround;
				break;
		}


		if (rawData.homeId == 0 && _inTownScene)
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
		
		GD.Print($"{rawData.name} has decided {rawData.currentState} as their state");
	}

	void GatherResources()
	{
		SetNavMeshAgentPath(false, TownManager.GetTownPlayerTravel(this).GlobalPosition);
		
		if (GlobalPosition.DistanceTo(_targetPosition) < 100)
		{
			_taskStarted = true;
			ToggleCollisionAndVisuals(false);
			_resourceTaskCounter++;
		}
	}

	void ResourceGatheringDone()
	{
		Random rnd = new Random();
		ToggleCollisionAndVisuals(true);
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
		SetNavMeshAgentPath(false,GlobalPosition + CreateOffsetVector2(-10, 10));
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
			SetNavMeshAgentPath(false, _currentPlant.GlobalPosition);
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
		SetNavMeshAgentPath(true, _player.GlobalPosition);
	}
	
	void FixBuildings()
	{
		if (_currentBuilding != null && VillagerManager.villagerManagerInstance.allBuildings.Contains(_currentBuilding))
		{
			if (!_currentBuilding.isDamaged)
			{
				_currentBuilding = null;
			}
			else
			{
				SetNavMeshAgentPath(false, _currentBuilding.GlobalPosition);

				if (GlobalPosition.DistanceTo(_currentBuilding.GlobalPosition) < 200)
				{
					_currentBuilding.FixBrokenBuilding();
				}
			}
		}
		else
		{
			var archerTowers = VillagerManager.villagerManagerInstance.GetDamagedBuildingsOfType(BuildingType.ArcherTower);
			var fences = VillagerManager.villagerManagerInstance.GetDamagedBuildingsOfType(BuildingType.Fence);
			var houses = VillagerManager.villagerManagerInstance.GetDamagedBuildingsOfType(BuildingType.House);

			if (archerTowers.Count > 0)
			{
				_currentBuilding = GetRandomDamagedBuilding(archerTowers);
			}
			else if (fences.Count > 0)
			{
				_currentBuilding = GetRandomDamagedBuilding(fences);
			}
			else if (houses.Count > 0)
			{
				_currentBuilding = GetRandomDamagedBuilding(houses);
			}
			else
			{
				SetCurrentState(VillagerState.RoamAround);
			}
		}
	}

	BuildingHealth GetRandomDamagedBuilding(List<BuildingHealth> buildings)
	{
		var randomUint = GD.Randi() % buildings.Count;
		
		return buildings[(int)randomUint];
	}

	


	void SoldierDuty()
	{
		if (!TimeManager.dayTime || VillagerManager.villagerManagerInstance.GetAllBrokenBuildings().Count > 0)
		{
			// It's fighting time!
			
			if (_archerTower == null || _archerTower.isBroken)
			{
				// Find a new tower!
				ExitArcherTower();
				_archerTower = FindFreeArcherTower();

				if (_archerTower is null)
				{
					// Run for your lives, no free towers
					SetCurrentState(VillagerState.FindShelter);
				}
			}
			else
			{
				// Man the tower!
				if (_archerTower.isOccupied)
				{
					if (_archerTower.occupyingVillagerId == rawData.id) return;
					
					// Somebody else manned it first...
					_archerTower = null;
				}

				SetNavMeshAgentPath(true, _archerTower.GlobalPosition);

				if (!(GlobalPosition.DistanceTo(_archerTower.GlobalPosition) < 200)) return;
                
				EnterArcherTower();
			}
		}
		else
		{
			// It's peaceful
			
			ExitArcherTower();
			SetCurrentState(VillagerState.RoamAround);
		}
    }
	
	
	ArcherTower FindFreeArcherTower()
	{
		var unoccupiedArcherTowers = VillagerManager.villagerManagerInstance.GetArcherTowerList()
			.Where(archerTower => !archerTower.isOccupied).ToArray();
		
		
		if (!unoccupiedArcherTowers.Any()) return null;
		
		int randomIndex = (int)(GD.Randi() % unoccupiedArcherTowers.Length);
		return unoccupiedArcherTowers[randomIndex];
	}
	
    
	void EnterArcherTower()
	{
		ToggleCollisionAndVisuals(false);
		_archerTower?.ActivateTower(rawData.id);
	}
	
	void ExitArcherTower()
	{
		_archerTower?.DeactivateTower();
		_archerTower = null;
		ToggleCollisionAndVisuals(true);
	}
}


