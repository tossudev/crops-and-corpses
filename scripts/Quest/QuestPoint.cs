
using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

public partial class QuestPoint : Node2D
{
	private QuestManager questManager;

	VillagerManager villagerManager;

	const string Node2D_questController = "";
	private QuestController questController;
	private ZombieManager zombieManager;

	  
	const string Node2D_ZombiePoint = "ZombieSpawn";
	private Node2D spawnZombiePoint;

	  
		
	const string Node2D_VillagerPoint = "VillagerSpawn";
	private Node2D villagerSpawnPoint;
	private PackedScene zombieScene;
	private PackedScene villagerScene;

	const string Area2D_ZombieArea = "ZombieArea";
	private Area2D ZombieArea;

	PlayerController playerController;

	private int zombieAmount = 4;
	private int villagerAmount = 1;
	private bool isQuestPointActive = false;

	int playerDistanceToQuestPoint;
	int SpawnRange = 100;

	int villagerSpawnRange = 50;

	int CurrentDifficulty;
	bool ifSpawned = false;


	public override void _Ready()
	{
		questController = GetNode<QuestController>("/root/QuestController");
		zombieManager = GetNode<ZombieManager>("/root/ZombieManager");
		spawnZombiePoint = GetNode<Node2D>("SpawnZombiePoint");
		zombieScene = (PackedScene)GD.Load("res://LilianTests/Prefabs/zombie_with_hitbox.tscn");
		villagerScene = (PackedScene)GD.Load("res://scenes/villager/villager.tscn");
		playerController = GetNode<PlayerController>("/root/PlayerController");
	 

		
	}

	public override void _Process(double delta)
	{

		//playerDistanceToQuestPoint = (int)playerController.GlobalPosition.DistanceTo(GlobalPosition);
		if (isQuestPointActive && !ifSpawned && playerDistanceToQuestPoint < SpawnRange)
		{
			SpawendZombieAmount();
			SpawnZombies();
			ifSpawned = true;
		}


	  

	}

	public void SpawnVillager()
	{
	
		{
		   GD.Print("villagerSpawnes");
		   
		}
	}

	public void SpawnZombies()
	{
		if (isQuestPointActive)
		{
			for (int i = 0; i < zombieAmount; i++)
			{
				CharacterBody2D zombie = (CharacterBody2D)zombieScene.Instantiate();
				zombie.Position = spawnZombiePoint.Position;
				GetTree().CurrentScene.AddChild(zombie);

			}
		}
	}

	private void SpawendZombieAmount(){
	   CurrentDifficulty = questManager.SelectedDifficulty;
	   switch (CurrentDifficulty)
	   {
		   case 1:
			   zombieAmount = 4;
			   break;
		   case 2:
			   zombieAmount = 6;
			   break;
		   case 3:
			   zombieAmount = 8;
			   break;
		   default:
			   zombieAmount = 4;
			   break;
	   }
	}


	public void On_ZombieArea_body_entered(Node body)
	{
		// if no zombies are in the area
		if (ZombieArea.GetOverlappingBodies().Count == 0)
		{
		  SpawnVillager();
		  questManager.CompleteQuestStage("Find");
	
		}
	}
}




	
		

   
