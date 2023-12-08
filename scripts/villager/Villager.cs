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
	PlayerSpriteController _spriteControl;
	Vector2 _targetPosition;
	Timer _taskTimer;
	Timer _chooseTaskTimer;
	[Export] NavigationAgent2D navMeshAgent;
	PackedScene villagerSkeleton;
	public Plant _currentPlant;
	public ArcherTower _currentArcherTower;
	public BuildingHealth _currentBuilding;
    
	AnimationPlayer _villagerAnimation;
	const string ANIMATION_PLAYER_NODENAME = "%VillagerAnimationPlayer";

	CharacterBody2D _player;

	
	float _speed = 100;
	bool _taskStarted = false;
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

		if (!(quest?.HasStage(QuestStage.Find) ?? false)) return;
		
		if (quest.HasStage(QuestStage.Kill)) return;
		
		if (!quest.CompleteQuestStage(QuestStage.Rescue)) return;
		quest.ChangeQuestDescription("Take the villager to Street Sign");

		VillagerManager.villagerManagerInstance.RescueAllVillagers();
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
				SetNavMeshAgentPath(false,GlobalPosition + CreateOffsetVector2(-200, 200));

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

		// Just making sure that there are no loose ends
		switch (rawData.currentOccupation)
		{
			case VillagerOccupation.Builder:
				if (decision != VillagerState.FixBuildings && _currentBuilding != null)
				{
					_currentBuilding = null;
				}
				break;
			case VillagerOccupation.Farmer:
				if (decision != VillagerState.FarmingTask && _currentPlant != null)
				{
					_currentPlant = null;
				}
				break;
			
			case VillagerOccupation.Soldier:

				if (decision != VillagerState.SoldierDuty && _currentArcherTower != null)
				{
					if (IsArcherTowerOccupiedByMe())
					{
						ExitArcherTower();
					}
				}
				
				break;
			case VillagerOccupation.Miner:
				break;
			case VillagerOccupation.Woodcutter:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		
		
		SetCurrentState(decision);
		
		GD.Print($"{rawData.name} has decided {rawData.currentState} as their state");
	}


	Timer _gatheringTimer;
	void GatherResources()
	{
		if (_taskStarted) return;
		
		SetNavMeshAgentPath(false, TownManager.GetTownPlayerTravel(this).GlobalPosition);
			
		if (GlobalPosition.DistanceTo(_targetPosition) < 100)
		{
			_taskStarted = true;

			_gatheringTimer = new Timer()
			{
				WaitTime = 15f,
				Autostart = true,
			};

			_gatheringTimer.Timeout += ResourceGatheringDone;
			AddChild(_gatheringTimer);
			
			ToggleCollisionAndVisuals(false);
		}



	}

	async void ResourceGatheringDone()
	{
		if (_gatheringTimer != null)
		{
			_gatheringTimer.QueueFree();
			_gatheringTimer = null;
		}
		
		Random rnd = new Random();

		RawInventoryItem foundItem;
		int amount = 0;
		
		switch (_rawData.currentOccupation)
		{
			case VillagerOccupation.Miner:
			{
				if (rnd.Next(0, 1) == 0)
				{
					Item foundStone = ItemData.GetItemById(5);
					
					amount = rnd.Next(4, 15);
					foundItem = foundStone.ItemAsRaw(amount);
				}
				else
				{
					Item foundCopper = ItemData.GetItemById(2);
					
					amount = rnd.Next(2, 8);
                    foundItem = foundCopper.ItemAsRaw(amount);
				}
				
				break;
			}
			case VillagerOccupation.Woodcutter:
			{
				Item foundWood= ItemData.GetItemById(0);
				
				amount = rnd.Next(4, 16);
				foundItem = foundWood.ItemAsRaw(amount);
				break;
			}
			
			default:
				foundItem = null;
				break;
		}

		await TownStorageController.AddItemToTownStorage(foundItem);
		
		_taskStarted = false;
		ToggleCollisionAndVisuals(true);
		ChooseTask();
	}

	void WaitingRescue()
	{
		StandStill();
	}

	void CheckPlants()
	{
		if (_currentPlant == null)
		{
			var plants = FarmManager.instance.GetPlantsThatNeedAttention();

			_currentPlant = plants.Count > 0
				? plants[(int) (GD.Randi() % plants.Count)]
				: null;

			if (_currentPlant == null)
			{
				// No plant found
				SetCurrentState(VillagerState.RoamAround);
			}
        }
		else
		{
			_currentPlant.isTendedTo = true;

			SetNavMeshAgentPath(false, _currentPlant.GlobalPosition);
				
			if (GlobalPosition.DistanceTo(_currentPlant.GlobalPosition) > 200) return;

			var growthState = _currentPlant.GetGrowthState();
				
			switch (growthState)
			{
				case GrowthState.WaitWatering or GrowthState.IsWilting:
					_currentPlant.WaterPlant();
					break;
				case GrowthState.IsInfested:
					_currentPlant.CurePlant();
					break;
			}
				
			_currentPlant = null;
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
			
			if (_currentArcherTower == null || _currentArcherTower.isBroken)
			{
				// Find a new tower!
				ExitArcherTower();
				_currentArcherTower = FindFreeArcherTower();

				if (_currentArcherTower is null)
				{
					// Run for your lives, no free towers
					SetCurrentState(VillagerState.FindShelter);
				}
			}
			else
			{
				// Man the tower!
				if (_currentArcherTower.isOccupied)
				{
					if (IsArcherTowerOccupiedByMe()) return;
					
					// Somebody else manned it first...
					_currentArcherTower = null;
					return;
				}

				SetNavMeshAgentPath(true, _currentArcherTower.GlobalPosition);

				if (!(GlobalPosition.DistanceTo(_currentArcherTower.GlobalPosition) < 200)) return;
                
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
		_currentArcherTower?.ActivateTower(rawData.id);
	}

	bool IsArcherTowerOccupiedByMe()
	{
		if (_currentArcherTower is null || rawData is null) return false;
		
		return _currentArcherTower.isOccupied && _currentArcherTower.occupyingVillagerId == rawData.id;
	}
	
	void ExitArcherTower()
	{
		_currentArcherTower?.DeactivateTower();
		_currentArcherTower = null;
		ToggleCollisionAndVisuals(true);
	}
}


