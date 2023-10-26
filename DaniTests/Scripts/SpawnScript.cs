using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class SpawnScript : Node2D
{
	Node2D[] spawnPoints;
	Timer spawnDelay;
	Timer zombieDeleteDelay;
	PackedScene packedScene;
	TimeManager dayTimeCheck;
	NodePath rootPath;
	Node2D rootNode;
	bool isNightOrDay;
	string isNightOrDayString;
	private int counter;

	private bool zombieDelayBool=false;
	[Export]private float maxDistance=1500f;
	[Export] public CharacterBody2D player;
	private static List<CharacterBody2D> zombieList = new List<CharacterBody2D>();
	public override void _Ready()
	{
		
		zombieList.Clear();
		counter = 0;
		spawnPoints = new Node2D[4];
		spawnDelay =GetNode<Timer>("Timer");
		zombieDeleteDelay = GetNode<Timer>("ZombieDeletionTimer");
		
		
		for(int i = 0; i < spawnPoints.Length; i++)
		{
			spawnPoints[i] = GetNode<Node2D>("SpawnPoint"+i);
			GD.Print(spawnPoints[i]);
		}
		
		packedScene = (PackedScene)GD.Load("res://LilianTests/Prefabs/zombie_with_hitbox.tscn");
		dayTimeCheck = GetNode<TimeManager>("SunlightContainer");
		spawnDelay.Start();
		GD.Print(zombieList.Count);
	}
  
	  public override void _Process(double delta)
    {
       
	   isNightOrDay = dayTimeCheck.dayTime;
	   
        
        if (!spawnDelay.IsStopped() && isNightOrDay)
        {
            spawnDelay.Start();
        }
        else if (spawnDelay.IsStopped() && !isNightOrDay)
        {
            spawnDelay.Stop();
        }
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
	private void DeleteZombieDelay()
	{
		//if(player.IsQueuedForDeletion()){zombieDeleteDelay.Stop();}
		zombieDelayBool = true;
	}
	private void ZombieSpawn()
	{

		rootPath =  GetParent<Node2D>().GetPath();
		GD.Print(rootPath);
		rootNode = GetNodeOrNull<Node2D>(rootPath);
		CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
		prefab.Position = spawnPoints[counter].Position;
		rootNode.AddChild(prefab);
		zombieList.Add(prefab);
		//GetNode<Node2D>("/root/Town").AddChild(prefab);
		counter ++;
		if(counter == spawnPoints.Length)
		{
			counter = 0;
		}
	}
	public static void RemoveZombieFromList(CharacterBody2D zombie)
	{
		zombieList.Remove(zombie);
	}
}
