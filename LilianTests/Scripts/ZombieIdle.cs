using Godot;
using System;

public partial class ZombieIdle : States
{
	[Export]
	public CharacterBody2D zombie;

	[Export]
	public float move_speed = 10.0f;

	private Vector2 move_direction = Vector2.Zero;
	private double roam_time;
	private CharacterBody2D player;
	
	public override void Enter()
    {
        player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
		RandomizeRoam();
    }

    public override void Update(double delta)
    {
        if(roam_time > 0)
		{
			roam_time -= delta;
		}
		else 
		{
			RandomizeRoam();
		}
    }

    public override void Physics_Update(double delta)
    {
        if(zombie != null)
		{
			zombie.Velocity = move_direction * move_speed;
		}
		
		if(player != null)
		{
			Vector2 direction = player.GlobalPosition - zombie.GlobalPosition;

			if(direction.Length() < 30 )
			{
				GD.Print("CHASETAAN");
				EmitSignal("Transitioned", "chase");			
			}
		}
		
    }

	private void RandomizeRoam()
	{
		/*move_direction = new Vector2(
			GD.Randf() % 2 - 1, // randf_range(-1, 1)
     		GD.Randf() % 2 - 1
		);*/

		move_direction = new Vector2(
			(float)(GD.RandRange(0, 2001) - 1000) / 1000,
			(float)(GD.RandRange(0, 2001) - 1000) / 1000 // random range -1, 1
		);

		// roam_time = GD.Randf() % 3 - 1;
		roam_time = (float)(GD.RandRange(1000, 3001)) / 1000; // random range 1, 3

		//GD.Print("MOVE_DIR: " + move_direction + "\nROAM_TIME: " + roam_time);
	}
}
