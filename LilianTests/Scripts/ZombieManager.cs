using Godot;
using System;

public partial class ZombieManager : Node
{
	[Export] public Node2D _villageTarget;
	public static Node2D moveTarget;
	public static bool dayMode;
	public static float damage;
	public static double attackTime;
	public static float idleSpeed;
	public static float chaseSpeed;
	private CharacterBody2D _player;
	public static bool playerAlive = true; 
	private SpawnScript _dayNightSpawnNode;
	private Timer _timer;

	public override void _Ready()
	{
		_dayNightSpawnNode = GetParent().GetNodeOrNull<SpawnScript>("NightDayCycleAndZombieSpawn");
		_timer = GetNodeOrNull<Timer>("Timer");
		if(_villageTarget != null)
		{
			moveTarget = _villageTarget;
		}
		_timer.Start();
	}
	public override void _Process(double delta)
	{
		// true = day, false = night
		dayMode = _dayNightSpawnNode.GetIsNightOrDay();			
	}

	private void CheckIfPlayerAlive()
	{
		if (playerAlive)
		{
			_player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
			if(_player == null)
			{
				playerAlive = false;
			}
		}
	}

	private void UpdateStats()
	{
		if (dayMode)
		{
			damage = 5f;
			attackTime = 1.0;
			idleSpeed = 100f;
			chaseSpeed = 150f;
		}
		else {
			damage = 10f;
			attackTime = 0.5;
			idleSpeed = 150f;
			chaseSpeed = 200f;
		}
	}

	private void OnTimerTimeout()
	{
		UpdateStats();	
	}
}
