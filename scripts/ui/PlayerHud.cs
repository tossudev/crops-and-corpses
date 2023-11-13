using Godot;
using System;

public partial class PlayerHud : Control {
	
	public CharacterBody2D player;

	string _tempLoseText = "get fucked looooll";


    public override void _Ready() {
		player = GetTree().GetNodesInGroup("player")[0] as CharacterBody2D;
    }


    public override void _PhysicsProcess(double delta)
    {
	    base._PhysicsProcess(delta);
	    
	    _UpdateHud();
    }


    void _UpdateHud() {
	    
	}
}
