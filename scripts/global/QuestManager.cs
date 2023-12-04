using Godot;
using System.Collections.Generic;

public partial class QuestManager : Node
{
	GlobalTime globalTime;

	private Quest activeQuest;
	
	public override void _Ready()
	{
		

		globalTime = GetNodeOrNull<GlobalTime>("/root/GlobalTime");

	
		
		if (globalTime == null)
		{
			GD.PrintErr("GlobalTime not found or not initialized.");
			// Handle the error as needed, e.g., return or throw an exception.
			return;
		}
	}


	void StartQuest(string questName, int difficulty, QuestType type, Scene.RootScene Location)
	{
		int StartDay = globalTime.GetDay();
		GD.Print($"Start day: {StartDay}");
		

		


		if (activeQuest == null)
		{
			
			Quest newQuest = new Quest(questName, difficulty, StartDay, type, Location);
			
			SetActiveQuest(newQuest);
		}
		else
		{
			GD.Print("Quest already active");
		}
	}
	

	public void StartRescueQuest(Scene.RootScene location, int difficulty)
	{
		if(difficulty > 0){
	
			for (int i = 0; i < difficulty; i++)
			{
				VillagerManager.villagerManagerInstance.AddNewVillagerRawData();
			}
		
			StartQuest($"Rescue Quest: {location.Name}", difficulty, QuestType.Rescue, location);
		

			GD.Print($"Rescue Quest started at {location.Name} with difficulty {difficulty}");
	}}
	
	
	public static void LoadQuest()
	{
		// Your implementation here
	}


	public Quest GetActiveQuest()
	{
		return activeQuest;
	}

	public void SetActiveQuest(Quest quest)
	{ 
		activeQuest = quest;
	} 

	




	
	public void CompleteQuestStage(string stage) => activeQuest?.CompleteQuestStage(stage);


	public void FinishQuest()
	{
		TownManager.GainExp(activeQuest.difficulty switch
		{
			1 => ExpGain.BIG,
			2 => ExpGain.VERY_BIG,
			3 => ExpGain.HUGE,
			_ => ExpGain.MEDIUM
		});
		
		SetActiveQuest(null);
	}
}
