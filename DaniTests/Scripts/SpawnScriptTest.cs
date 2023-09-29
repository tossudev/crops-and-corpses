using Godot;
using System;

public partial class SpawnScriptTest : Node2D
{

	PackedScene packedScene;
	TimeManager dayTimeCheck;
	bool isNightOrDay;

	public override void _Ready()
	{
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
				string isNightOrDayString = isNightOrDay ? "DayTime" : "NighTime";
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
}
