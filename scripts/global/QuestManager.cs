using Godot;
using System.Collections.Generic;

public partial class QuestManager : Node
{
	GlobalTime globalTime;

	private Quest activeQuest;
	
	public override void _Ready()
	{
		

		globalTime = GetNodeOrNull<GlobalTime>("/root/GlobalTime");

	
		StartRescueQuest(Scene.Cave,1);
		if (globalTime == null)
		{
			GD.PrintErr("GlobalTime not found or not initialized.");
			// Handle the error as needed, e.g., return or throw an exception.
			return;
		}
	}


	void StartQuest(string questName, int difficulty, QuestType type, Scene.RootScene questLocation)
	{
		int StartDay = globalTime.GetDay();
		GD.Print($"Start day: {StartDay}");

		if (activeQuest == null)
		{
			Quest newQuest = new Quest(questName, difficulty, StartDay, type, questLocation);
			
			SetActiveQuest(newQuest);
		}
		else
		{
			GD.Print("Quest already active");
		}
	}
	

	public void StartRescueQuest(Scene.RootScene location, int difficulty)
	{
		StartQuest($"Rescue Quest: {location.Name}", difficulty, QuestType.Rescue, location);
	}

	public void CompleteQuest()
	{
		SetActiveQuest(null);
	}
	
	public static void LoadQuest()
	{
		// Your implementation here
	}


	public Quest GetActiveQuest() => activeQuest;

	public void SetActiveQuest(Quest quest)
	{ 
		activeQuest = quest;
	} 


	public void CompleteQuestStage(string stage) => activeQuest?.CompleteQuestStage(stage);
	
}
