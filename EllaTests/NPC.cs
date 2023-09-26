using Godot;
using System;

public partial class NPC : CharacterBody2D
{
    public override void _Ready()
    {
        
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton){
			GD.Print("NPC clicked!");
		}
    }
}
