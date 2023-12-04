using Godot;

public partial class ZombieManager : Node
{
	public static  ZombieType type;
	
	public static bool dayMode;
	public static int damage;
	public static double attackTime = 1;
	public static float idleSpeed = 100f;
	public static float chaseSpeed = 150f;
	private CharacterBody2D _player;
	public static bool playerAlive = true; 
	private SpawnScript _dayNightSpawnNode;
	private Timer _timer;
	public enum ZombieType {Weak,Medium,Strong};

	public static float zombieKillCount;
	static AudioController _audioController;	

	public override void _Ready()
	{
		_audioController = GetNode<AudioController>("/root/Audio");
		
		_dayNightSpawnNode = GetParent().GetNodeOrNull<SpawnScript>("ZombieSpawn");
		_timer = GetNodeOrNull<Timer>("Timer");

		_timer.Start();
	}
	public override void _Process(double delta)
	{
		// true = day, false = night
		dayMode = _dayNightSpawnNode.GetIsNightOrDay();		
	}

	public static void PlayZombieNoise(ZombieNoises noise)
	{
		_audioController.PlayEffect(noise.ToString());	
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
		int townStats = TownManager.currentTownStats.townHallLevel;
		if(zombieKillCount > 0) zombieKillCount -= 0.2f; // Decreasing kill count by 0.5 every second

		if (dayMode)type = ZombieType.Weak;
		else 
		{
			type = ZombieType.Medium;
			if(zombieKillCount > 10 && townStats > 4) type = ZombieType.Strong;	
		}
		
		switch(type)
		{	
			case ZombieType.Strong:
				damage = 15;
				attackTime = 0.5;
				idleSpeed = 200;
				chaseSpeed = 250;
				break;
			case ZombieType.Medium:
				damage = 10;
				attackTime = 0.5;
				idleSpeed = 150f;
				chaseSpeed = 200f;
				break;
			case ZombieType.Weak:
				damage = 5;
				attackTime = 1.0f;
				idleSpeed = 100f;
				chaseSpeed = 150f;
				break;
			default:
			break;
		}
		AddZombieKillCountUpgrades();
		AddZombieTownLevelUpgrades(townStats);
	}

	private void OnTimerTimeout()
	{
		UpdateStats();	
	}
	private void AddZombieTownLevelUpgrades(int townStats)
	{
		switch(townStats)
		{
			case 1:
			//Level 1 no buffs
				break;
			case 2:
				damage += 3;
				attackTime += 0.1;
				idleSpeed += 30;
				chaseSpeed += 30;
				break;
			case 3:
				damage += 5;
				attackTime += 0.3;
				idleSpeed += 40;
				chaseSpeed += 40;
				break;
			case 4:
				damage += 7;
				attackTime += 0.5;
				idleSpeed +=50;
				chaseSpeed +=50;
				break;
			default:
				break;
		}	
	}
	private void AddZombieKillCountUpgrades()
	{
		if(zombieKillCount > 10)
		{
				damage += 20;
				attackTime += 1;
				idleSpeed +=30;
				chaseSpeed +=30;
		}
	}
}
