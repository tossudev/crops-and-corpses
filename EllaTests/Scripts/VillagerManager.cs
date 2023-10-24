using Godot;
using System;
using System.Collections.Generic;

public partial class VillagerManager : Node
{
	[Export] PackedScene [] _villagerPrefab;
	public static VillagerManager instance;
	List<Villager> _villager = new List<Villager>();
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

	}

	public enum TaskType
	{
		FarmingTask,
		FindResourchesTask
	}

	public enum VillagerStates
	{
		RoamAround,
		FollowPlayer,
		FixFence,
		FindArcherTower,
		FindShelter,
		GetHospitalized,
		ChooseTask
	}
}
