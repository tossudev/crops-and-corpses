using Godot;
using System;

public partial class ZombieManager : Node
{
	public static  ZombieType type;
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
	public enum ZombieType {Weak,Medium,Strong};
	

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
			type= ZombieType.Weak;
		}
		else {
			float random = (float)GD.RandRange(0.0,1.0);
			if(random < 0.3f)
			{
				type=ZombieType.Strong;
			}
			else
			{
				type = ZombieType.Medium;
			}
		}
		switch(type)
		{	
			case ZombieType.Strong:
				
				damage = 15f;
				attackTime = 0.5;
				idleSpeed = 200;
				chaseSpeed = 250;
				break;
			case ZombieType.Medium:
				damage = 10f;
				attackTime = 0.5;
				idleSpeed = 150f;
				chaseSpeed = 200f;
				break;
			case ZombieType.Weak:
				damage = 5f;
				attackTime = 1.0f;
				idleSpeed = 100f;
				chaseSpeed = 150f;
				break;
			default:
			break;
		}
	
		
	}

	private void OnTimerTimeout()
	{
		UpdateStats();	
	}
}
