using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Collectables : Node
{
	[Export] private string _name;
	private bool _playerInArea;
	private int _amount;

	public override void _Ready()
	{		
		_playerInArea = false;
		_amount = 0;
	}
	public override void _Process(double delta)
	{
		if (_playerInArea)
		{
			if(Input.IsActionPressed("interact"))
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
		if(_name == "seeds")
		{
			_amount = random_amount.Next(1, 4);
			//PlayerInventoryData.AddItemToInventory(0, amount);
		}
		if(_name == "somethingElse")
		{
			_amount = 10;
		}

		GD.Print("collectable name: " + _name + ", amount: " + _amount);		
	}
}
