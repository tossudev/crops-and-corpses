
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

	
	const string Node2D_ZombiePoint = "%ZombieSpawn";
	private Node2D spawnZombiePoint;


	const string Node2D_QuestZombies = "%QuestZombies";
	private Node2D[] QuestZombies;

	  
		
	const string Node2D_VillagerPoint = "%VillagerSpawn";
	private Node2D villagerSpawnPoint;
	private PackedScene zombieScene;
	private PackedScene villagerScene;

	const string Area2D_ZombieArea = "%ZombieArea";
	private Area2D ZombieArea;

	PlayerController playerController;
	
	

	private int zombieAmount = 4;
	private int villagerAmount = 1;
	public bool isQuestPointActive = false;

	int playerDistanceToQuestPoint;
	int SpawnRange = 500;

	int villagerSpawnRange = 50;

	int CurrentDifficulty;
	bool isZombiesSpawned = false;


	public override void _Ready()
	{
		questManager = GetNode<QuestManager>("/root/QuestManager");

		QuestZombies = GetNode<Node2D[]>(Node2D_QuestZombies); 
		spawnZombiePoint = GetNode<Node2D>(Node2D_ZombiePoint);
		villagerSpawnPoint = GetNode<Node2D>(Node2D_VillagerPoint);
		playerController = (PlayerController)GetTree().GetFirstNodeInGroup("player");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if(isQuestPointActive == true)
		{
		playerDistanceToQuestPoint = (int)playerController.GlobalPosition.DistanceTo(GlobalPosition);

		GD.Print("Distance to quest point: " + playerDistanceToQuestPoint);
		if (isQuestPointActive && !isZombiesSpawned && playerDistanceToQuestPoint < SpawnRange)
		{
			SpawendZombieAmount();
			SpawnZombies();
			SpawnVillagers();
			isZombiesSpawned = true;
		}
		
		
			
		}

	
	}

	
	void SpawnVillagers( )
	{
		
		VillagerManager.villagerManagerInstance.SpawnQuestVillagers();

	}

	public void SpawnZombies()
	{
		zombieScene = (PackedScene) ResourceLoader.Load("res://scenes/zombie/Zombie.tscn");
		

		for (int i = 0; i < zombieAmount; i++)
		{
			CharacterBody2D zombie = (CharacterBody2D) zombieScene.Instantiate();
			zombie.GlobalPosition = spawnZombiePoint.GlobalPosition + new Vector2(GD.RandRange(-SpawnRange, SpawnRange), GD.RandRange(-SpawnRange, SpawnRange));
			QuestZombies[i].AddChild(zombie);


		}


	}

	private void SpawendZombieAmount(){
	   CurrentDifficulty = questManager.GetActiveQuest().difficulty;
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
		
	}

	public void ActivateQuestPoint()
	{
		
		isQuestPointActive = true;
	}
}




	
		

   
