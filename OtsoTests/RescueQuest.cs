using Godot;
using System;

public partial class RescueQuest : Node
{
	private QuestManager questManager;


	private SceneTree RescueTarget;

    public object Position { get; internal set; }

    public override void _Ready()
		{
			// Assuming the QuestManager is an autoload singleton
			
			questManager = GetNode<QuestManager>("/root/QuestManager");
			RescueTarget = GetNode<SceneTree>("ResqueTarget");
		
	    	
		}

		
	


	
	public void OnRescuePointAreaEntered(Area2D area)
	{
		GD.Print("Rescue point entered");
		questManager.CompleteQuest("Rescue mission");
		area.Visible = false;
	}
		


	



	
	
	
}
