using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Manages the active quest in the game.
/// </summary>


public partial class QuestManager : Node
{
	private Quest activeQuest;
	GlobalTime globalTime;

	public override void _Ready()
	{
		GD.Print($"Current Scene: {GetTree().CurrentScene.Name}");
		activeQuest = null;
		SetDifficulty(1);
		GD.Print($"Active quest: {activeQuest?.Name}");

		globalTime = GetNode<GlobalTime>("/root/GlobalTime");
		if (activeQuest == null)
		{

			
		
			StartForrestQuest();
			
			GD.Print($"Active quest: {activeQuest?.Name}");
			GD.Print($"Active quest difficulty: {activeQuest?.Difficulty}");
		}
		else
		{
			GD.Print("GlobalTime node not found");
		}
	}

	public void StartQuest(string questName, string questDescription, List<string> questStages, string questLocation)
	{
		int StartDay = globalTime.GetDay();
		GD.Print($"Start day: {StartDay}");
		if (activeQuest == null && StartDay < globalTime.GetDay())
		{
			Quest newQuest = new Quest
			{
				Name = questName,
				Description = questDescription,
				Stages = questStages,
				Location = questLocation
			};
			SetActiveQuest(newQuest);
		}
		else
		{
			GD.Print("Quest already active");
		}
	}

	public void StartForrestQuest()
	{
		List<string> questStages = new List<string> { "Find", "Rescue", "Deliver" };
		StartQuest("Forrest Quest", "Rescue Villager From Forrest", questStages, "Forrest");
		GD.Print("Forrest Quest Started");
		GD.Print($"Active quest: {activeQuest?.Name}");
	}

	public void StartRuinsQuest()
	{
		List<string> questStages = new List<string> { "Find", "Rescue", "Deliver" };
		StartQuest("Ruins Quest", "Rescue Villager From Ruins", questStages, "Ruins");
	}

	public void StartCaveQuest()
	{
		List<string> questStages = new List<string> { "Find", "Rescue", "Deliver" };
		StartQuest("Cave Quest", "Rescue Villager From cave", questStages, "Cave");
	}

	public void LoadQuest()
	{
		//load quest from save file

	}

	public Quest GetActiveQuest()
	{
		return activeQuest;
	}

	public void SetActiveQuest(Quest quest)
	{
		activeQuest = quest;
		GD.Print($"Active quest set: {quest?.Name}");
	}

	public int GetActiveQuestDifficulty()
	{
		return activeQuest?.Difficulty ?? 0;
	}

	public void CompleteQuestStage(string stage)
	{
		activeQuest?.CompleteQuestStage(stage);
	}

   public void SetDifficulty(int difficulty)
{
	if (activeQuest != null)
	{	
		activeQuest.Difficulty = difficulty;
		GD.Print($"Quest difficulty set to: {difficulty}");
	}
	else
	{
		GD.Print("No active quest to set difficulty for.");
	}
}
}



	


	
