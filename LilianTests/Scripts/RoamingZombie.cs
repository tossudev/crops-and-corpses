using Godot;
using System;

public partial class RoamingZombie : CharacterBody2D
{
	private Sprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
		
		if (Velocity.X > 0)
    	{
    		sprite.FlipH = false;
    	}
    	else
    	{
    		sprite.FlipH = true;
    	}
    }

    // // Get the gravity from the project settings to be synced with RigidBody nodes.
    // //public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    // enum ZOMBIE_STATE { IDLE, ROAM }

    // [Export]
    // public float move_speed = 20.0f;

    // [Export]
    // public float idle_time = 5.0f;

    // [Export]
    // public float walk_time = 2.0f;

    // private Timer timer;
    // private Vector2 move_direction = Vector2.Zero;
    // private ZOMBIE_STATE current_state = ZOMBIE_STATE.IDLE;
    // private Sprite2D sprite;


    // public override void _Ready()
    // {		
    // 	timer = GetNode<Timer>("Timer");
    // 	sprite = GetNode<Sprite2D>("Sprite2D");		
    //     SelectNewDirection();
    // 	PickNewState();		
    // }


    // public override void _PhysicsProcess(double delta)
    // {
    // 	// only move zombie when its in roam state
    // 	if(current_state == ZOMBIE_STATE.ROAM)
    // 	{
    // 		Velocity = move_direction * move_speed;
    // 		MoveAndSlide();
    // 	}
    // }

    // private void SelectNewDirection()
    // {
    // 	move_direction = new Vector2(
    // 		GD.Randi() % 3 - 1, // randi_range(-1, 1)
    // 		GD.Randi() % 3 - 1
    // 	);

    // 	if (move_direction.X < 0)
    // 	{
    // 		sprite.FlipH = true;
    // 	}
    // 	else if (move_direction.X > 0)
    // 	{
    // 		sprite.FlipH = false;
    // 	}
    // }

    // private void PickNewState()
    // {
    // 	if (current_state == ZOMBIE_STATE.IDLE)
    //     {            
    //         current_state = ZOMBIE_STATE.ROAM;
    // 		SelectNewDirection();
    // 		timer.Start(walk_time);			
    //     }
    //     else if (current_state == ZOMBIE_STATE.ROAM)
    //     {            
    //         current_state = ZOMBIE_STATE.IDLE;
    // 		timer.Start(idle_time);
    //     }
    // }

    // private void _on_timer_timeout()
    // {
    // 	//GD.Print("TIME");
    // 	PickNewState();
    // }
}
