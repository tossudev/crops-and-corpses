using Godot;
using System;

public partial class GlobalTime : Node
{
	private float globalTime=0f;
	private Color sunlight;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public float GetTime()
	{
		return globalTime;
	}
	public void SetTime(float time)
	{
		globalTime = time;
	}
	public Color GetColor()
	{
		return sunlight;
	}
	public void SetColor(Color color)
	{
		sunlight = color;
	}
	
}
