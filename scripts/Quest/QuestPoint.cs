
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


	const string Node2D_ZombieSpawn = "%ZombieSpawn";
	private Node2D zombieSpawn;

	
	const string Node2D_QuestZombieSpawn = "%QuestZombieSpawn";
	private Node2D spawnZombiePoint;


	const string Node2D_QuestZombies = "%QuestZombies";
	private Node2D QuestZombies;

	  
		
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
		base._Ready();
		QuestZombies = GetNode<Node2D>(Node2D_QuestZombies);
	

		questManager = GetNode<QuestManager>("/root/QuestManager");

		zombieScene = (PackedScene)ResourceLoader.Load("res://scenes/zombie/Zombie.tscn");
		spawnZombiePoint = GetNode<Node2D>(Node2D_QuestZombieSpawn);

		zombieSpawn = GetNode<Node2D>(Node2D_ZombieSpawn);
	

		

		

 

	

	playerController = (PlayerController)GetTree().GetFirstNodeInGroup("player");
	if (playerController == null)
	
		
	
		questManager.StartRescueQuest(Scene.Cave, 1);
		GD.Print(questManager.GetActiveQuest().difficulty);
	
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	

		if(isQuestPointActive == true)
		{
		playerDistanceToQuestPoint = (int)playerController.GlobalPosition.DistanceTo(GlobalPosition);

	
		if (isQuestPointActive && !isZombiesSpawned && playerDistanceToQuestPoint < SpawnRange)
		{
			
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

		GD.Print("SpawnZombies");
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




	
		

   
