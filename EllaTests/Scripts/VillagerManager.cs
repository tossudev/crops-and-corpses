using Godot;
using System;
using System.Collections.Generic;

public partial class VillagerManager : Node
{
	CharacterBody2D _villagerPrefab;
	Villager _newVillager;
	public static VillagerManager instance;
	List<Villager> _villager = new List<Villager>();
	int _villagerCount;
	public int villagerMaxAmount;
	public int townhallLevel = 0;
	// Called when the node enters the scene tree for the first time.

	[Export] AllVillagerData _allData;
	public override void _Ready()
	{
		if(instance==null)instance=this;else QueueFree();
		townhallLevel = 0;
	}

	public VillagerData GetVillagerData(){
		VillagerData data = new VillagerData();
		data.name = _allData.GetName();
		data.info = _allData.GetInfo();
		data.texture = _allData.GetTexture();

		return data;

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void FindVillagers()
	{
 	/* 	_villagerPrefab = GetNode<CharacterBody2D>("Enemies/NPC");
		_villagerPrefab = _newVillager;

		for(int i = 0; i < _villagerPrefab.Count; i++)
		{
			 _villager.Add(_newVillager);
		} */ 

	}

	void VillagerAmountInGame()
	{
		switch(townhallLevel)
		{
			case 0:
			villagerMaxAmount = 4;
			break;

			case 1:
			villagerMaxAmount = 5;
			break;

			case 2:
			villagerMaxAmount = 6;
			break;
		}
	}

	public struct VillagerData{
		public string name;
		public string info;
		public Texture2D texture;
	}

	public enum VillagerStates
	{
		RoamAround,
		FollowPlayer,
		FixFence,
		FindArcherTower,
		FindShelter,
		GetHospitalized,
		ChooseTask,
		FarmingTask,
		FindResourchesTask
	}
}
