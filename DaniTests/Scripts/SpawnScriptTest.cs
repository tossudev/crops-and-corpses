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
	private bool spawnCheck = true;

	public override void _Ready()
	{
		spawnPoints = new Node2D[3];
		spawnDelay =GetNode<Timer>("Timer");
		
		for(int i = 0; i < 3; i++)
		{
			spawnPoints[i] = GetNode<Node2D>("SpawnPoint"+i);
			GD.Print(spawnPoints[i]);
		}
		
		packedScene = (PackedScene)GD.Load("res://LilianTests/Prefabs/roaming_zombie.tscn");
		dayTimeCheck = GetNode<TimeManager>("SunlightContainer");
	}
    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventMouseButton mouseEvent)
		{
			if(mouseEvent.IsActionPressed("left_click"))
			{
				CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
				prefab.Position = mouseEvent.Position;
				this.AddChild(prefab);
				
				isNightOrDay = dayTimeCheck.returnTimeOfDay(isNightOrDay);
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
    }
	public override void _Process(double delta)
	{
		isNightOrDay = dayTimeCheck.returnTimeOfDay(isNightOrDay);
		isNightOrDayString = isNightOrDay ? "DayTime" : "NighTime";
		if(isNightOrDayString == "DayTime" && spawnCheck)
		{
			spawnCheck = false;
			GD.Print("hei");
			spawnDelay.Timeout += ZombieSpawn;

		}
	}
	private void ZombieSpawn()
	{
		
		for(int i = 0; i < 3; i++)
		{
			GD.Print("Hoi");
			CharacterBody2D prefab = (CharacterBody2D)packedScene.Instantiate();
			prefab.Position = spawnPoints[i].Position;
			AddChild(prefab);
		
		}
		spawnCheck = true;
	}
}
