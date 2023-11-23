using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class VillagerManager : Node
{
	public static VillagerManager instance;
	
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
    

	[Export] AllVillagerData _allData;

	const string VILLAGER_SCENE_PATH = "res://scenes/villager/villager.tscn";

	Node2D _villagerParentNode;
	const string VILLAGER_PARENT_NODEPATH = "%Villagers";
	
	public override void _Ready()
	{
		if(instance==null)instance=this;else QueueFree();

		_villagerParentNode = GetNode<Node2D>(VILLAGER_PARENT_NODEPATH);
		
		if (SceneManager.IsCurrentScene(this, Scene.Town))
		{
			SpawnSavedVillagers();
		}
	}

	async void SpawnSavedVillagers()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);

		foreach (var villagerRawData in SaveData.allVillagerData)
		{
			AddExistingVillager(villagerRawData);
		}
		
		bool test;
		test = AddNewVillager();
		test = AddNewVillager();
	}
	
	public VillagerData GetNewVillagerData(){
		
		return new VillagerData
		{
			name = _allData.GetName(),
			info = _allData.GetInfo(),
			texture = _allData.GetTexture()
		};
	}

	public bool AddNewVillager(bool intoTown = true)
	{
		if (intoTown && _allVillagers.Count >= TownManager.currentTownStats.populationCap) return false;
		
		Villager newVillager = GD.Load<PackedScene>(VILLAGER_SCENE_PATH).Instantiate<Villager>();
        
		VillagerData newData = GetNewVillagerData();
		VillagerRawData newRawData = new VillagerRawData(newData.name, newData.info, intoTown);
		
		SaveData.allVillagerData.Add(newRawData);
		
		RegisterAndInitVillager(newVillager, newRawData);
		return true;
	}

	public void AddExistingVillager(VillagerRawData existingVillagerRawData)
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
	}
}

public struct VillagerData{
	public string name;
	public string info;
	public Texture2D texture;
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
	ResqueQuest
}
