using Godot;
using System;
using System.Collections.Generic;

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
    
	// Called when the node enters the scene tree for the first time.

	[Export] AllVillagerData _allData;
	public override void _Ready()
	{
		if(instance==null)instance=this;else QueueFree();
	}

	public VillagerData GetVillagerData(){
		VillagerData data = new VillagerData();
		data.name = _allData.GetName();
		data.info = _allData.GetInfo();
		data.texture = _allData.GetTexture();

		return data;

	}

	public bool AddNewVillager(Villager newVillager, bool intoTown = true)
	{
		if (intoTown && _allVillagers.Count >= TownManager.currentTownStats.populationCap) return false;
		
		_allVillagers.Add(newVillager);

		VillagerData newData = GetVillagerData();
		VillagerRawData newRawData = new VillagerRawData(newData.name, newData.info, intoTown);
		
		SaveData.allVillagers.Add(newRawData);
		newVillager.InitializeVillager(newRawData);
		return true;
	}

	public void AddExistingVillager(Villager existingVillager, VillagerRawData data)
	{
		_allVillagers.Add(existingVillager);
		existingVillager.InitializeVillager(data);
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

	public struct VillagerData{
		public string name;
		public string info;
		public Texture2D texture;
	}
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
