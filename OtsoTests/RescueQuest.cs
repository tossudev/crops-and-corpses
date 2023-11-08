using Godot;
using System;

public partial class RescueQuest : Node
{
	private QuestManager questManager;
	private Node2D[] RescuePoints;

	private SceneTree RescueTarget;

	


		public override void _Ready()
		{
			// Assuming the QuestManager is an autoload singleton
			
			questManager = GetNode<QuestManager>("/root/QuestManager");
			RescueTarget = GetNode<SceneTree>("ResqueTarget");
	    	RandomPoint();
		}
	


	public void RandomPoint(){
		Random random = new Random();
		int randomIndex = random.Next(RescuePoints.Length);
		Node2D randomPoint = RescuePoints[randomIndex];
		
		
		
		
		
		GD.Print("Random point: " + randomPoint);
	}

	public void OnRescuePointAreaEntered(Area2D area)
	{
		GD.Print("Rescue point entered");
		questManager.CompleteQuest("Rescue mission");
		area.Visible = false;
	}
		


	



	
	
	
}
