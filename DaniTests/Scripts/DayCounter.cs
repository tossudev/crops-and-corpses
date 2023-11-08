using Godot;
using System;

public partial class DayCounter : Label
{
	private int currentDay;
	private bool isDayTime;
	
	private GlobalTime globalTime;
	[Export] TimeManager timeManager;

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
		isDayTime = timeManager.dayTime;
		//GD.Print(timeManager.dayTime);
		if(timeManager != null)
		{
			isDayTime = timeManager.dayTime;

		}
		if(isDayTime)
		{
			this.Modulate=new Color(1f,1f,1f);
			
			
		} 
		else
		{
			this.Modulate = new Color(1f,0,0); //Change color from white to red when night comes.
		} 

		currentDay = globalTime.GetDay();
		Text = "Day: "+currentDay.ToString();
		
	}
}
