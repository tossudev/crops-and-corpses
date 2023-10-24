using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class SpawnScriptTest : Node2D
{
	Node2D[] spawnPoints;
	Timer spawnDelay;
	PackedScene packedScene;
	TimeManager dayTimeCheck;
	NodePath rootPath;
	Node2D rootNode;
	bool isNightOrDay;
	string isNightOrDayString;
	private int counter;
	[Export]private float maxDistance=1500f;
	[Export] public CharacterBody2D player;
	private List<CharacterBody2D> zombieList = new List<CharacterBody2D>();
	public override void _Ready()
	{
		
		counter = 0;
		spawnPoints = new Node2D[4];
		spawnDelay =GetNode<Timer>("Timer");
		
		
		for(int i = 0; i < spawnPoints.Length; i++)
		{
			spawnPoints[i] = GetNode<Node2D>("SpawnPoint"+i);
			GD.Print(spawnPoints[i]);
		}
		
		packedScene = (PackedScene)GD.Load("res://LilianTests/Prefabs/zombie_with_hitbox.tscn");
		dayTimeCheck = GetNode<TimeManager>("SunlightContainer");
		spawnDelay.Start();
	}
    /* public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventMouseButton mouseEvent)
		{
			if(mouseEvent.IsActionPressed("left_click"))
			{
				CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
				prefab.Position = mouseEvent.Position;
				this.AddChild(prefab);
				isNightOrDay = dayTimeCheck.dayTime;
				isNightOrDayString = isNightOrDay ? "DayTime" : "NighTime";
				if(isNightOrDayString == "DayTime")
				{
					GD.Print("It is Day!");
				}
				else
				{
					GD.Print("It is Night!");
				}
			}
		}
    } */
	  public override void _Process(double delta)
    {
       // isNightOrDay = dayTimeCheck.returnTimeOfDay(isNightOrDay);
	   isNightOrDay = dayTimeCheck.dayTime;
	   
        
        if (!spawnDelay.IsStopped() && isNightOrDay)
        {
            spawnDelay.Start();
        }
        else if (spawnDelay.IsStopped() && !isNightOrDay)
        {
            spawnDelay.Stop();
        }
		if(zombieList != null)
		{
			Vector2 playerPos = player.Position;
			for(int i = zombieList.Count-1;i>=0;i--)
			{
				Vector2 zombiePos = zombieList[i].Position;

				float distance = zombiePos.DistanceTo(playerPos);
				//GD.Print("Distance to player" + distance+ " MaxDistance "+maxDistance);
				if(distance > maxDistance)
				{
					zombieList[i].QueueFree();
					zombieList.RemoveAt(i);
					
				}



			}
		}
    }
	private void ZombieSpawn()
	{

		rootPath =  GetParent<Node2D>().GetPath();
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
	public void RemoveZombieFromList(CharacterBody2D zombie)
	{
		zombieList.Remove(zombie);
	}
}
