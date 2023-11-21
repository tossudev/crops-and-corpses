
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the quest points in the game.
/// </summary>
public partial class QuestPointManager : Node
{ private Dictionary<string, PackedScene> questScenesByScene = new Dictionary<string, PackedScene>();
    private List<QuestPoint> activeQuestPoints = new List<QuestPoint>();
    private QuestManager questManager;

    public override void _Ready()
    {
        questManager = GetNode<QuestManager>("/root/QuestManager");

        // Load and store PackedScenes for each quest type
        questScenesByScene["forest"] = GD.Load<PackedScene>("res://path/to/forest_quest_scene.tscn");
        questScenesByScene["ruins"] = GD.Load<PackedScene>("res://path/to/ruins_quest_scene.tscn");
        questScenesByScene["cave"] = GD.Load<PackedScene>("res://path/to/cave_quest_scene.tscn");
    }

    public void AddQuestPoint(QuestPoint questPoint, string sceneName)
    {
        // Add the quest point to the list of active quest points
        activeQuestPoints.Add(questPoint);
    }

    public Vector2 GetRandomQuestPoint(string sceneName)
    {
        // Your existing code to get a random quest point
        // ...

        return Vector2.Zero; // Placeholder, replace with your logic
    }

    public void ActivateQuestPointForScene(string sceneName)
    {
        Vector2 randomQuestPoint = GetRandomQuestPoint(sceneName);

        if (randomQuestPoint != Vector2.Zero && questManager.GetActiveQuest() == null)
        {
            // Load the PackedScene for the specific quest type
            if (questScenesByScene.TryGetValue(sceneName, out PackedScene questScene))
            {
                // Instantiate the quest from the PackedScene
                Quest newQuest = (Quest)questScene.Instantiate();
                newQuest.Position = randomQuestPoint;
                GetTree().CurrentScene.AddChild(newQuest);

                GD.Print($"Quest instantiated at random position: {randomQuestPoint}");

                // Assign the quest to the QuestPoint or perform other actions
                QuestPoint questPoint = newQuest.GetNode<QuestPoint>(".");
                questPoint.AssignQuest(newQuest);
            }
            else
            {
                GD.PrintErr($"PackedScene not found for scene: {sceneName}");
            }
        }
    }

    // Example method to deactivate a specific quest point
    public void DeactivateQuestPoint(QuestPoint questPoint)
    {
        // Deactivate or perform other actions related to deactivation
        questPoint.QueueFree(); // Assuming QuestPoint is a Node2D, change as needed
        activeQuestPoints.Remove(questPoint);
    }
}
