using Godot;
using System;

public partial class HourCounter : Label
{
	private float currentHour;
	private bool isDayTime;
	private GlobalTime globalTime;
	private float dayDuration = 300.0f;
	private float nightDuration = 150.0f;
	


	private float hourOfDay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalTime = GetNode<GlobalTime>("/root/GlobalTime");
		currentHour = globalTime.GetTime();
		hourOfDay = MathF.Floor(currentHour / (dayDuration + nightDuration) * 24+6) % 24;
		Text ="Hour: "+ currentHour.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(SceneManager.GetCurrentScene(this) == Scene.Cave)
		{
			Modulate = new Color(1f,0,0);
		}
		else
		{
			isDayTime = TimeManager.dayTime;
			Modulate = isDayTime 
				? new Color(1f,1f,1f) 
				: new Color(1f,0,0);
		}
		
		currentHour = globalTime.GetTime();
		hourOfDay = MathF.Floor(currentHour / (dayDuration + nightDuration) * 24+6) % 24;
		if(hourOfDay == 0)
		{
			hourOfDay = 24;
		}
		Text = "Hour: "+hourOfDay.ToString();
		
	}
}
