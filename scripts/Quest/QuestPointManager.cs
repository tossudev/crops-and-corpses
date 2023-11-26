
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
      
    }
   

   public Vector2 GetRandomQuestPoint(Node2D[] QuestPoint)
    {
        Random random = new Random();
        int randomQuestPointIndex = random.Next(QuestPoint.Length);
        Vector2 randomQuestPoint = QuestPoint[randomQuestPointIndex].Position;
        return randomQuestPoint;
    }
    public void ActivateQuestPointForScene(string sceneName)
    {
        // Get a random quest point for the scene
        Vector2 randomQuestPoint = GetRandomQuestPoint(QuestPoint);
    
    
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
