using Godot;

public partial class ZombieManager : Node
{
	public static  ZombieType type;
	[Export] public Node2D _villageTarget;
	public static Node2D moveTarget;
	public static bool dayMode;
	public static int damage;
	public static double attackTime;
	public static float idleSpeed;
	public static float chaseSpeed;
	private CharacterBody2D _player;
	public static bool playerAlive = true; 
	private SpawnScript _dayNightSpawnNode;
	private Timer _timer;
	public enum ZombieType {Weak,Medium,Strong};

    static AudioController _audioController;
	

	public override void _Ready()
	{
		_audioController = GetNode<AudioController>("/root/Audio");
		
		_dayNightSpawnNode = GetParent().GetNodeOrNull<SpawnScript>("ZombieSpawn");
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
	
		
	}

	private void OnTimerTimeout()
	{
		UpdateStats();	
	}
}
