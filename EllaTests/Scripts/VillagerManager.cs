using Godot;
using System;
using System.Collections.Generic;

public partial class VillagerManager : Node
{
	CharacterBody2D _villagerPrefab;
	public static VillagerManager instance;
	List<Villager> _villager = new List<Villager>();
	int _villagerCount;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void FindVillagers()
	{
/* 		_villagerPrefab = GetNode<CharacterBody2D>("Enemies/NPC");

		for(int i = 0; i < _villager.Count; i++)
		{
			 
		} */

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
