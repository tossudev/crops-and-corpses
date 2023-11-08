using Godot;
using System;


public partial class QuestBoard : Node2D
{
	private QuestManager questManager;


	RescueQuest rescueQuest;

    bool isquestActive = false;

    public override void _Ready()
    {
        // Assuming the QuestManager is an autoload singleton
        questManager = GetNode<QuestManager>("/root/QuestManager");
		rescueQuest = GetNode<RescueQuest>("RescueQuest");
		
    }

    public  override void _PhysicsProcess(double delta)
	
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            GD.Print("Quest board button pressed");
            questManager.CreateRescueMission("Otso");

        }
    }

    public void onQuestBoardButtonPressed()
    {
        GD.Print("Quest board button pressed");

        questManager.R

        questManager.CreateRescueMission("Otso");
    }

    // Call this method when the player presses the quest board button

        
}
