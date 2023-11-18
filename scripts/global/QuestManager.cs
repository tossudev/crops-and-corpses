using Godot;
using System;
using System.Collections.Generic;

public partial class QuestManager : Node
{
    private Quest activeQuest = null;
    private string currentSceneName = "";

    public override void _Ready()
    {
        // Initialize quests
        InitializeQuests();
    }

    private void InitializeQuests()
    {
        // Add quests to the list with stages
        List<string> stages1 = new List<string> { "Collect 5 items", "Deliver items to NPC" };
        List<string> stages2 = new List<string> { "Talk to NPC", "Retrieve an item" };

        Quest quest1 = new Quest();
        quest1.Initialize("Quest 1", "Help villagers", stages1, "forest");

        Quest quest2 = new Quest();
        quest2.Initialize("Quest 2", "Explore dungeon", stages2, "ruins");

        SetActiveQuest(quest1); // Set the initial active quest
    }

    public void SetActiveQuest(Quest quest)
    {
        activeQuest = quest;
        currentSceneName = activeQuest.SceneName;
    }

    public void SetActiveQuestForScene(string sceneName)
    {
        // Assuming this method is called when the player enters a new scene
        if (activeQuest != null && activeQuest.SceneName != sceneName)
        {
            // Reset the current quest if it's not for the new scene
            activeQuest.ResetQuest();
            activeQuest = null;
        }

        if (activeQuest == null)
        {
            // Initialize a new quest for the current scene
            List<string> newQuestStages = new List<string> { "Stage 1", "Stage 2", "Stage 3" };
            Quest newQuest = new Quest();
            newQuest.Initialize("New Quest", "Quest description", newQuestStages, sceneName);
            SetActiveQuest(newQuest);
            activeQuest.StartQuest(); // Start the quest immediately when initialized
        }
    }

    public Quest GetActiveQuest()
    {
        return activeQuest;
    }

    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
}
