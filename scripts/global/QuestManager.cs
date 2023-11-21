using Godot;
using System;
using System.Collections.Generic;

public partial class QuestManager : Node
{
    private Quest activeQuest;

    public void SetActiveQuestForScene(string sceneName)
    {
        // Implement logic to set the active quest based on the scene (if needed)
        // For simplicity, let's assume there's only one global quest for now.
        // You can extend this logic to support different quests for different scenes.
        if (activeQuest == null)
        {
            List<string> questStages = new List<string> { "Stage 1", "Stage 2", "Stage 3" };
            Quest newQuest = new Quest();
            newQuest.Initialize("Global Quest", "Global quest description", questStages, sceneName);
            SetActiveQuest(newQuest);
        }
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
}
