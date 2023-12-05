using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class SpawnScript : Node2D
{
	private partial class SpawnPoint : Node2D
	{
		public Node2D Node { get; set; }
		//public bool IsActive { get; set; }
	}
	List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
	Timer spawnDelay;
	Timer zombieDeleteDelay;
	PackedScene packedScene;

	Node2D enemiesNode;
	bool isNightOrDay;
	string isNightOrDayString;
	private int counter;
	private int spawnPointCount;
	private bool zombieDelayBool=false;
	[Export]private float maxDistance;
	[Export] public CharacterBody2D player;
	private static List<CharacterBody2D> zombieList = new List<CharacterBody2D>();
	private static List<CharacterBody2D> questZombieList = new List<CharacterBody2D>();
	public GlobalTime globalTime;

	public override void _Ready()
	{
		globalTime = GetNode<GlobalTime>("/root/GlobalTime");
		zombieList.Clear();
		counter = 0;
		foreach (Node node in GetChildren())
		{
			if(node is Node2D spawnPoint && node.Name.ToString().Contains("SpawnPoint"))
			{
				spawnPoints.Add(new SpawnPoint{Node = spawnPoint});
				spawnPointCount +=1;
				
			//	GD.Print(spawnPoint);
			}
		}
		//GD.Print(spawnPointCount);
		spawnDelay =GetNode<Timer>("Timer");
		zombieDeleteDelay = GetNode<Timer>("ZombieDeletionTimer");
		packedScene = (PackedScene)GD.Load("res://scenes/zombie/Zombie.tscn");
		spawnDelay.Start();
		
	}
  
	  public override void _Process(double delta)
	{
	   CheckIfInsideCave();
		if (!spawnDelay.IsStopped() && isNightOrDay)
		{
			spawnDelay.Start();
		}
		else if (spawnDelay.IsStopped() && !isNightOrDay)
		{
			spawnDelay.Stop();
		}
		CheckIfTownDestroyed();
		if(zombieList.Count > 0 && zombieDelayBool)
		{
			Vector2 playerPos = player.Position;
			zombieDelayBool = false;
			for(int i = zombieList.Count-1;i>=0;i--)
			{
				Vector2 zombiePos = zombieList[i].Position;
				float distance = zombiePos.DistanceTo(playerPos);
				//GD.Print("Distance to player" + distance+ " MaxDistance "+maxDistance);
				if(distance > maxDistance)
				{
					GD.Print("Zombie Deleted");
					zombieList[i].QueueFree();
					zombieList.RemoveAt(i);
				}
			}
		}
	}
	private void CheckIfTownDestroyed()
	{
		if(globalTime.HasTownBeenDestroyed())
		{
			if(GetParent<Node2D>().Name != "Cave") spawnDelay.Stop();
			
		}
	}
	private void DeleteZombieDelay()
	{
		//if(player.IsQueuedForDeletion()){zombieDeleteDelay.Stop();}
		zombieDelayBool = true;

	}
	private void CheckIfInsideCave()
	{
		if (GetParent<Node2D>().Name != "Cave")
		{
			isNightOrDay = TimeManager.dayTime;
		}
		else
		{
			isNightOrDay = false;
		}
	 
	}
	public void ZombieSpawn()
{
    enemiesNode = GetNode<Node2D>("%Enemies");

    foreach (SpawnPoint spawnPoint in spawnPoints)
    {
        Vector2 spawnPointPos = spawnPoint.Node.Position;
        Vector2 playerPos = player.Position;

        float distance = spawnPointPos.DistanceTo(playerPos);
        GD.Print(distance + " spawnPoint distance to player");

        if (distance <= maxDistance)
        {
            CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
            prefab.Position = spawnPointPos;
            enemiesNode.AddChild(prefab);
            zombieList.Add(prefab);
        }
        else
        {
            GD.Print("Spawn point too far away, skipping spawn.");
        }
    }
}
	public void SpawnZombieAtPoint(Vector2 spawnPoint)
	{    

		enemiesNode = GetNode<Node2D>("%Enemies");

		CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
		prefab.Position = spawnPoint;
		enemiesNode.AddChild(prefab);
		questZombieList.Add(prefab);
	}
	


/* 	private SpawnPoint FindClosestActiveSpawnPoint()
	{
		SpawnPoint closestSpawnPoint = null;
		float closestDistance = float.MaxValue;

		foreach (SpawnPoint sp in spawnPoints.Where(sp => sp.IsActive))
		{
			float distance = sp.Node.Position.DistanceTo(player.Position);
			if (distance < closestDistance)
			{
				closestSpawnPoint = sp;
				closestDistance = distance;
			}
		}

		return closestSpawnPoint;
	} */
	public static void RemoveZombieFromList(CharacterBody2D zombie)
	{
		zombieList.Remove(zombie);

		if (questZombieList.Contains(zombie))
		{
			questZombieList.Remove(zombie);
		}

		
		
	}

	public bool GetIsNightOrDay()
	{
		return isNightOrDay;
	}

    public int GetZombieQuestListCount()
    {
        return questZombieList.Count;
    }




    /* public void QuestZombieSpawn(int spawnAmount)
{	
    for(int y = 0; y < spawnPointCount; y++)
    {
        ZombieSpawn();
    }
} */
}
