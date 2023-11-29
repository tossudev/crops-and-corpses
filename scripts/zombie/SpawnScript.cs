using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class SpawnScript : Node2D
{
	List<Node2D> spawnPoints = new List<Node2D>();
	Timer spawnDelay;
	Timer zombieDeleteDelay;
	PackedScene packedScene;
	PackedScene packedScene2;
	PackedScene packedScene3;
	NodePath rootPath;
	Node2D rootNode;
	bool isNightOrDay;
	string isNightOrDayString;
	private int counter;
	private int spawnPointCount;
	private bool zombieDelayBool=false;
	[Export]private float maxDistance=1500f;
	[Export] public CharacterBody2D player;
	private static List<CharacterBody2D> zombieList = new List<CharacterBody2D>();
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
				spawnPoints.Add(spawnPoint);
			//	GD.Print(spawnPoint);
			}
		}
		spawnDelay =GetNode<Timer>("Timer");
		zombieDeleteDelay = GetNode<Timer>("ZombieDeletionTimer");
		packedScene = (PackedScene)GD.Load("res://scenes/zombie/Zombie.tscn");
		spawnDelay.Start();
	}
  
	  public override void _Process(double delta)
    {
	   isNightOrDay = TimeManager.dayTime;
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
			spawnDelay.Stop();
		}
	}
	private void DeleteZombieDelay()
	{
		//if(player.IsQueuedForDeletion()){zombieDeleteDelay.Stop();}
		zombieDelayBool = true;
	}
	private void CheckIfInsideCave()
	{
		if(GetParent<Node2D>().Name == "Cave")
	   {	
			isNightOrDay = false;
	   }
	 
	}
	public void ZombieSpawn()
	{
		rootPath =  GetParent<Node2D>().GetPath();
		rootNode = GetNodeOrNull<Node2D>(rootPath);
		CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
		prefab.Position = spawnPoints[counter].Position;
		rootNode.AddChild(prefab);
		zombieList.Add(prefab);
		counter ++;
		if(counter == spawnPoints.Count)
		{
			counter = 0;
		}
	}
	public static void RemoveZombieFromList(CharacterBody2D zombie)
	{
		zombieList.Remove(zombie);
	}

	public bool GetIsNightOrDay()
	{
		return isNightOrDay;
	}
}
