using Godot;
using System;
using System.Collections.Generic;

public partial class QuestManager : Node
{
	GlobalTime globalTime;
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


	async void StartQuest(int difficulty, QuestType type, Scene.RootScene Location)
	{
		int StartDay = globalTime.GetDay();
		GD.Print($"Start day: {StartDay}");
		
        
		if (await PlayerInfo.GetActiveQuest() == null)
		{
			
			Quest newQuest = new Quest(difficulty, StartDay, type, Location);
			
			PlayerInfo.SetActiveQuest(newQuest);
		}
		else
		{
			GD.Print("Quest already active");
		}
	}
	

	public async void StartRescueQuest(Scene.RootScene location, int difficulty)
	{
		if (difficulty <= 0) return;
		
		if (await PlayerInfo.GetActiveQuest() != null) return;
		
		for (int i = 0; i < difficulty; i++)
		{
			VillagerManager.villagerManagerInstance.AddNewVillagerRawData();
		}

		StartQuest(difficulty, QuestType.Rescue, location);


		GD.Print($"Rescue Quest started at {location.Name} with difficulty {difficulty}");
	}


	public async void FinishQuest()
	{
		var quest = await PlayerInfo.GetActiveQuest();
		
		if (quest == null)
		{
			GD.PushError("Can't finish a null quest");
			return;
		}
		
		TownManager.GainExp(quest.questDifficulty switch
		{
			1 => ExpGain.BIG,
			2 => ExpGain.VERY_BIG,
			3 => ExpGain.HUGE,
			_ => ExpGain.MEDIUM
		});
		
		PlayerInfo.SetActiveQuest(null);
	}
}
