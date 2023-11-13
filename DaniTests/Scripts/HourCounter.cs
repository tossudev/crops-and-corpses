using Godot;
using System;

public partial class HourCounter : Label
{
	private float currentHour;
	
	private GlobalTime globalTime;
	private float dayDuration = 60.0f;
	private float nightDuration = 30.0f;


	private float hourOfDay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalTime = GetNode<GlobalTime>("/root/GlobalTime");
		currentHour = globalTime.GetTime();
		hourOfDay = MathF.Floor(currentHour / (dayDuration + nightDuration) * 24) % 24;
		Text ="Hour: "+ currentHour.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		currentHour = globalTime.GetTime();
		hourOfDay = MathF.Floor(currentHour / (dayDuration + nightDuration) * 24) % 24;
		Text = "Hour: "+hourOfDay.ToString();
		
	}
}
