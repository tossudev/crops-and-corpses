using Godot;
using System;
using System.Collections.Generic;

public partial class QuestController : Node2D
{
    private QuestManager questManager;

    public override void _Ready()
    {
        questManager = GetNode<QuestManager>("/root/QuestManager");

        // Simulate player entering a scene (you can replace this with actual scene transition logic)
        EnterScene("forest");
    }

    private void EnterScene(string sceneName)
    {
        questManager.SetActiveQuestForScene(sceneName);
        ActivateRandomQuestPoint(sceneName);
    }
 private void ActivateRandomQuestPoint(string sceneName)
    {
        // Check if the active quest matches the current scene
        Quest activeQuest = questManager.GetActiveQuest();
        if (activeQuest != null && activeQuest.SceneName == sceneName)
        {
            // Activate a random quest point in the current scene
            Vector2 randomQuestPoint = questPointManager.GetRandomQuestPoint(sceneName);

            // Instantiate a quest at the random point
            PackedScene questScene = GD.Load<PackedScene>("res://path/to/your/quest_scene.tscn");
            questPointManager.InstantiateQuestAtPoint(questScene, randomQuestPoint);
        }

    
    }

    private void InitQuest()
    {
        // Initialize a new quest (you can customize this based on your specific implementation)
        Quest newQuest = new Quest();
        List<string> questStages = new List<string> { "Stage 1", "Stage 2", "Stage 3" };
        newQuest.Initialize("New Quest", "Quest description", questStages, questManager.GetCurrentSceneName());

        // Set the newly initialized quest as the active quest
        questManager.SetActiveQuest(newQuest);
    }
}
    