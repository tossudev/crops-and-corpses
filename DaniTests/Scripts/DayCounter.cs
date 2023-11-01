using Godot;
using System;

public partial class DayCounter : Label
{
	private int currentDay;
	
	private GlobalTime globalTime;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalTime = GetNode<GlobalTime>("/root/GlobalTime");
		currentDay = globalTime.GetDay();
		Text ="Day: "+ currentDay.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		currentDay = globalTime.GetDay();
		Text = "Day: "+currentDay.ToString();
		
	}
}
