using Godot;
using System;

public partial class HouseHealth : Node2D
{[Export]
	public HealthComponent healthComponent;
	[Export]
	 public ProgressBar healthBar;

	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthBar.MaxValue = healthComponent.health;
		healthBar.Value = healthComponent.health;
		
	}


	public override void _Process(double delta)
	{
		if( healthComponent.health == healthComponent.GetMaxHealth())
		{
			healthBar.Visible = false;
		}
		else
		{
			healthBar.Visible = true;
		}
		healthBar.Value = healthComponent.health;


		
	}

}
