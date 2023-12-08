using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class VillagerManager : Node2D
{
	public static VillagerManager villagerManagerInstance;
	
	// Villager Lists
	List<Villager> _allVillagers = new ();
	
	List<Villager> _builder = new ();
	public List<Villager> BuilderVillagers => _builder;
	
	List<Villager> _farmers = new ();
	public List<Villager> farmerVillagers => _farmers;

	List<Villager> _soldiers = new ();
	public List<Villager> soldierVillagers => _soldiers;

	List<Villager> _woodcutters = new ();
	public List<Villager> woodcutterVillagers => _woodcutters;
	
	List<Villager> _miners = new ();
	public List<Villager> minerVillagers => _miners;

	List<BuildingHealth> _allBuildings = new ();
	public List<BuildingHealth> allBuildings => _allBuildings;
    

	[Export] AllVillagerData _allData;
	public AllVillagerData allVillagerData => _allData;

	const string VILLAGER_SCENE_PATH = "res://scenes/villager/villager.tscn";

	Node2D _villagerParentNode;
	const string VILLAGER_PARENT_NODEPATH = "%Villagers";

	public readonly List<VillagerResidence> allVillagerResidences = new ();

	
	public override void _Ready()
	{
		if(villagerManagerInstance==null)villagerManagerInstance=this;else QueueFree();

		_villagerParentNode = GetNode<Node2D>(VILLAGER_PARENT_NODEPATH);
		
		if (SceneManager.IsCurrentScene(this, Scene.Town))
		{
			SpawnSavedVillagers();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (TownManager.EveryXSecond((int) AutosaveIntervalSeconds.VILLAGER_POSITION_INTERVAL))
		{
			SaveVillagers();
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		villagerManagerInstance = null;
	}

	async void SpawnSavedVillagers()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);

		foreach (var villagerRawData in SaveData.allVillagerRawData)
		{
			if(villagerRawData.isTownPopulation)
			{
				SpawnExistingVillager(villagerRawData);
			}
		}
	}

	public VillagerRawData AddNewVillagerRawData(bool intoTown = false)
	{
		VillagerRawData newRawData = new VillagerRawData(_allData.GetName(), _allData.GetInfo(), intoTown, Vector2.Zero);
		SaveData.allVillagerRawData.Add(newRawData);

		return newRawData;
	}
	
	public bool SpawnNewVillager(Vector2 spawnCoordinates, bool intoTown = true)
	{
		if (intoTown && _allVillagers.Count >= TownManager.currentTownStats.populationCap) return false;
        
		RegisterAndInitVillager(
			GD.Load<PackedScene>(VILLAGER_SCENE_PATH).Instantiate<Villager>(),
			AddNewVillagerRawData(intoTown),
			spawnCoordinates
			);
		
		return true;
	}

	public void SpawnExistingVillager(VillagerRawData existingVillagerRawData)
	{
		if (SaveData.allVillagerRawData.All(data => data.id != existingVillagerRawData.id))
		{
			GD.PushError("Villager data not found");
			return;
		}
		
		
		
		RegisterAndInitVillager(
			GD.Load<PackedScene>(VILLAGER_SCENE_PATH).Instantiate<Villager>(),
			existingVillagerRawData,
			new Vector2(existingVillagerRawData.xCoord, existingVillagerRawData.yCoord)
			);
	}

	public void SpawnQuestVillagers(Vector2 position)
	{
		SaveData.allVillagerRawData.ForEach(data =>
		{
			Vector2 offsetVector = new Vector2(GD.Randi() % 3, GD.Randi() % 3);
			
			if (!data.isTownPopulation)
			{
				data.SetCoordinates(position + offsetVector);
				SpawnExistingVillager(data);
			}
		});
	}
	
	void RegisterAndInitVillager(Villager villagerToRegister, VillagerRawData data, Vector2 spawnCoordinates)
	{
		if (_allVillagers.All(villager => villager.rawData.id != data.id))
		{
			_allVillagers.Add(villagerToRegister);
		}
		
		_villagerParentNode.AddChild(villagerToRegister);
		
		
		villagerToRegister.InitializeVillager(data);
		
		villagerToRegister.Teleport(spawnCoordinates);
	}

    void SaveVillagers()
	{
		foreach (var villager in _allVillagers)
		{
			villager.SavePosition();
		}

		Task save = SaveData.SyncVillagers();
	}
	
	public void SetVillagerOccupation(Villager villager, VillagerOccupation newOccupation)
	{
		if (villager == null)
		{
			GD.PushError("Can't set occupation for null @VillagerManager");
			return;
		}

		if (villager.currentOccupationList != null && villager.currentOccupationList.Contains(villager))
		{
			villager.currentOccupationList?.Remove(villager);
		}

		villager.currentOccupationList = newOccupation switch
		{
			VillagerOccupation.Builder => _builder,
			VillagerOccupation.Farmer => _farmers,
			VillagerOccupation.Soldier => _soldiers,
			VillagerOccupation.Woodcutter => _woodcutters,
			VillagerOccupation.Miner => _miners,
			_ => throw new ArgumentOutOfRangeException(nameof(newOccupation), newOccupation, null)
		};
		
		villager.currentOccupationList.Add(villager);

		villager.rawData.currentOccupation = newOccupation;
		
        villager.skeleton.ChangeHat(newOccupation);
		villager.ChooseTask();
	}

	public Texture2D GetTextureByType(VillagerType type, BodyPartTextureType part)
	{
		return _allData.GetTextureByType(type, part);
	}

	public List<BuildingHealth> GetDamagedBuildingsOfType(BuildingType buildingType)
	{
		return _allBuildings.FindAll(building => building.isDamaged && building.buildingType == buildingType);
	}
	
	public List<BuildingHealth> GetAllBrokenBuildings()
	{
		return _allBuildings.FindAll(building => building.isBroken);
	}

	public List<ArcherTower> GetArcherTowerList()
	{
		List<ArcherTower> archerTowerList = new List<ArcherTower>();
		var archerTowers = _allBuildings.FindAll(building => !building.isDamaged && building.buildingType == BuildingType.ArcherTower);
		archerTowers.ForEach(archerTower => archerTowerList.Add((ArcherTower)archerTower.GetParent()));
		return archerTowerList;
	}

	public void AddNewBuilding(BuildingHealth newBuilding)
	{
		_allBuildings.Add(newBuilding);
		TownHallStatsPanel._thStatsPanelInstance?.UpdateStat(TownStatType.BROKEN_BUILDINGS);
	}

	public void RemoveBuilding(BuildingHealth buildingToRemove)
	{
		_allBuildings.Remove(buildingToRemove);
		TownHallStatsPanel._thStatsPanelInstance?.UpdateStat(TownStatType.BROKEN_BUILDINGS);
	}

	public void AddNewResidence(VillagerResidence residence)
	{
		allVillagerResidences.Add(residence);
        UpdateHousingInfo();
	}
	
	public void RemoveResidence(VillagerResidence residence)
	{
		allVillagerResidences.Remove(residence);
		UpdateHousingInfo();
	}

	void UpdateHousingInfo()
	{
		int housesProvided = 0;
		allVillagerResidences.ForEach(residence => housesProvided += residence.housingCapacity);
		
		TownManager.currentTownStats.providedHomes = housesProvided;
			
		
		TownHallStatsPanel._thStatsPanelInstance?.UpdateStat(TownStatType.HOUSING);
	}

	public VillagerResidence FindResidenceById(int id)
    {
        return allVillagerResidences.Find(residence => residence.id == id);
    }
	
	public List<VillagerResidence> GetFreeHomesList()
	{
		return allVillagerResidences.FindAll(
			residence => residence.hasRoomForMoreVillagers && !residence.isBroken);;
	}

	public void RescueAllVillagers()
	{
		SaveData.allVillagerRawData.FindAll(data => !data.isTownPopulation)
			.ForEach(rescuedVillager => rescuedVillager.currentState = VillagerState.FollowPlayer);
	}
}

public enum VillagerType
{
	Female1,
	Female2,
	Female3
}

public enum BodyPartTextureType
{
	Head,
	Body,
	LeftArm,
	RightArm,
	LeftFoot,
	RightFoot
}

public enum VillagerState
{
	RoamAround,
	FollowPlayer,
	FixBuildings,
	SoldierDuty,
	FindShelter,
	InShelter,
	FarmingTask,
	FindWoodTask,
	FindStoneTask,
	RescueQuest,
	Homeless
}
