using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class VillagerManager : Node
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
	public List<ArcherTower> archerTowerList = new List<ArcherTower>();
	public List<BuildingHealth> _brokenFenceList = new List<BuildingHealth>();
    

	[Export] AllVillagerData _allData;

	const string VILLAGER_SCENE_PATH = "res://scenes/villager/villager.tscn";

	Node2D _villagerParentNode;
	const string VILLAGER_PARENT_NODEPATH = "%Villagers";
	
	public override void _Ready()
	{
		if(villagerManagerInstance==null)villagerManagerInstance=this;else QueueFree();

		_villagerParentNode = GetNode<Node2D>(VILLAGER_PARENT_NODEPATH);
		
		if (SceneManager.IsCurrentScene(this, Scene.Town))
		{
			SpawnSavedVillagers();
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

		foreach (var villagerRawData in SaveData.allVillagerData)
		{
			if(villagerRawData.isTownPopulation)
			{
				SpawnExistingVillager(villagerRawData);
			}
		}
		
		bool test;
		test = SpawnNewVillager();
		test = SpawnNewVillager();
	}
	
	public VillagerData GetNewVillagerData(){
		
		return new VillagerData
		{
			name = _allData.GetName(),
			info = _allData.GetInfo(),
			texture = _allData.GetTexture()
		};
	}

	public VillagerRawData AddNewVillagerRawData(bool intoTown = false)
	{
		VillagerData newData = GetNewVillagerData();
		VillagerRawData newRawData = new VillagerRawData(newData.name, newData.info, intoTown);
		SaveData.allVillagerData.Add(newRawData);

		return newRawData;
	}
	
	public bool SpawnNewVillager(bool intoTown = true)
	{
		if (intoTown && _allVillagers.Count >= TownManager.currentTownStats.populationCap) return false;
        
		RegisterAndInitVillager(
			GD.Load<PackedScene>(VILLAGER_SCENE_PATH).Instantiate<Villager>(),
			AddNewVillagerRawData(intoTown));
		
		return true;
	}

	public void SpawnExistingVillager(VillagerRawData existingVillagerRawData)
	{
		RegisterAndInitVillager(
			GD.Load<PackedScene>(VILLAGER_SCENE_PATH).Instantiate<Villager>(), existingVillagerRawData);
	}

	void RegisterAndInitVillager(Villager villagerToRegister, VillagerRawData data)
	{
		if (_allVillagers.All(villager => villager.rawData.id != data.id))
		{
			_allVillagers.Add(villagerToRegister);
		}
		
		_villagerParentNode.AddChild(villagerToRegister);
		villagerToRegister.InitializeVillager(data);
	}

	public void SetVillagerOccupation(Villager villager, VillagerOccupation newOccupation)
	{
		if (villager == null)
		{
			GD.PushError("Can't set occupation for null @VillagerManager");
			return;
		}
		villager.currentOccupationList?.Remove(villager);

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
		villager.villagerInfo.ChangeHat(newOccupation);
	}

	public Texture2D GetTextureByType(VillagerType type, BodyPartTextureType part)
	{
		return _allData.GetTextureByType(type, part);
	}
	public void AddArcherTower(ArcherTower archerTower)
	{
		archerTowerList.Add(archerTower);
	}
	public List<ArcherTower> GetArcherTowerList()
	{
		return archerTowerList;
	}
	void CountBrokenFencesInTown()
	{
		_brokenFenceList.Clear();
		var _fences = (Node2D)GetTree().GetFirstNodeInGroup("fences");

		foreach (Node2D fence in _fences.GetChild(0).GetChildren())
		{
			var fenceHealth = fence.GetNode<BuildingHealth>("%BuildingHealth");
			if(fenceHealth.isBroken)
			{
				_brokenFenceList.Add(fenceHealth);
			}
		}
	}
	public List<BuildingHealth> GetFenceList()
	{
		CountBrokenFencesInTown();
		return _brokenFenceList;
	}
}

public struct VillagerData{
	public string name;
	public string info;
	public Texture2D texture;
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
	FixFence,
	FindArcherTower,
	FindShelter,
	InShelter,
	ChooseTask,
	FarmingTask,
	FindWoodTask,
	FindStoneTask,
	RescueQuest
}
