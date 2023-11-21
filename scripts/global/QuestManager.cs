using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the active quest in the game.
/// </summary>
public partial class QuestManager : Node
{
    private Quest activeQuest;


     public override void _Ready(){

        
     }

    public void StartQuest(string questName, string questDescription, List<string> questStages, string questLocation)
    {
        if (activeQuest == null)
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
    }

    public void StartForrestQuest()
    {
        List<string> questStages = new List<string> { "Find", "Rescue", "Deliver" };
        StartQuest("Forrest Quest", "Rescue Villager From Forrest", questStages, "Forrest");
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
        GD.Print($"Active quest set: {quest.Name}");
    }

    public int GetActiveQuestDifficulty()
    {
        return activeQuest?.Difficulty ?? 0;
    }

    public void CompleteQuestStage(string stage)
    {
        activeQuest?.CompleteQuestStage(stage);
    }
}
