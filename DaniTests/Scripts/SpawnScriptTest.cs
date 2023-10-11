using Godot;
using System;
using System.Collections;
using System.Linq;

public partial class SpawnScriptTest : Node2D
{
	Node2D[] spawnPoints;
	Timer spawnDelay;
	PackedScene packedScene;
	TimeManager dayTimeCheck;
	bool isNightOrDay;
	string isNightOrDayString;
	private int counter;
	public override void _Ready()
	{
		counter = 0;
		spawnPoints = new Node2D[3];
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
    }
	private void ZombieSpawn()
	{
		
		CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
		prefab.Position = spawnPoints[counter].Position;
		AddChild(prefab);
		counter ++;
		if(counter == 3)
		{
			counter = 0;
		}
	}
}
