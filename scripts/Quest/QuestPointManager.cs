
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

    public Node2D[] QuestPoint;

    


    public override void _Ready()
    {
        questManager = GetNode<QuestManager>("/root/QuestManager");

       

        // Load and store PackedScenes for each quest type
        questScenesByScene["forest"] = GD.Load<PackedScene>("res://path/to/forest_quest_scene.tscn");
        questScenesByScene["ruins"] = GD.Load<PackedScene>("res://path/to/ruins_quest_scene.tscn");
        questScenesByScene["cave"] = GD.Load<PackedScene>("res://path/to/cave_quest_scene.tscn");
    }

    public override void _Process(double delta)
    {
        // Check if the active quest has been completed
        if (questManager.GetActiveQuest() != null && questManager.GetActiveQuest().IsCompleted)
        {
            // Deactivate the quest point
            DeactivateQuestPoint(questManager.GetActiveQuest().QuestPoint);
        }
    }
    public Vector2 GetRandomQuestPoint(string sceneName)
    {
        // Get a random quest point from the list of active quest points
        QuestPoint QuestPoint = activeQuestPoints[GD.Randi() % activeQuestPoints.Count];
        
        Vector2 randomQuestPoint = QuestPoint.Position;
         return randomQuestPoint;
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
                Quest QuestPoint = (Quest)questScene.Instantiate();
                QuestPoint.Position = randomQuestPoint;
                GetTree().CurrentScene.AddChild(QuestPoint);

                GD.Print($"Quest instantiated at random position: {randomQuestPoint}");
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
