using Godot;
using System;

public partial class GlobalTime : Node
{
	private float globalTime=0f;
	private Color sunlight;
	private int day=1;


   

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
	public int GetDay()
	{
		return day;
	}
	public void SetDay(int currentDay)
	{
		day = currentDay;
		GD.Print(day);
	}
}
