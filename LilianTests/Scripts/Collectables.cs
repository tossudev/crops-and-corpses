using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Collectables : Node
{
	private string _name;
	private bool _playerInArea;
	private int _amount;

	public override void _Ready()
	{		
		_playerInArea = false;
		_amount = 0;
		_name = Name;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (_playerInArea)
		{
			if(Input.IsActionJustPressed("interact"))
			{
				CollectAmount();				
				QueueFree();
			}
		}
	}

	private void OnPickAreaEntered(Node2D body)
	{
		if(body.IsInGroup("player"))
		{
			_playerInArea = true;
			GD.Print(_name);
		}
	}

	private void OnPickAreaExited(Node2D body)
	{
		if(body.IsInGroup("player"))
		{
			_playerInArea = false;
		}
	}

	private void CollectAmount()
	{
		Random random_amount = new Random();
		if(_name == "collectable_seed")
		{
			_amount = random_amount.Next(1, 4);
			//PlayerInventoryData.AddItemToInventory(0, amount);
		}
		if(_name == "collectable_something")
		{
			_amount = 10;
		}

		GD.Print("collectable name: " + _name + ", amount: " + _amount);		
	}
}
