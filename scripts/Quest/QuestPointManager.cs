using Godot;
using System;
using System.Collections.Generic;

public partial class QuestPointManager : Node
{
	
    private List<QuestPoint> activeQuestPoints = new List<QuestPoint>();
    private QuestManager questManager;

    PackedScene forrestQuestScene = (PackedScene)GD.Load("res://scenes/Quest/ForrestQuest.tscn");
    PackedScene caveQuestScene = (PackedScene)GD.Load("res://scenes/Quest/CaveQuest.tscn");
    PackedScene villageQuestScene = (PackedScene)GD.Load("res://scenes/Quest/VillageQuest.tscn");

    PackedScene InThisSceneQuestScene;

    [Export]
    public Node2D[] QuestPoint;

    

   public Vector2 GetRandomQuestPoint(Node2D[] QuestPoint)
    {
        Random random = new Random();
        int randomQuestPointIndex = random.Next(QuestPoint.Length);
        Vector2 randomQuestPoint = QuestPoint[randomQuestPointIndex].Position;
        return randomQuestPoint;
    }
    public void ActivateQuestPointForScene(string sceneName)
    {
        GetRandomQuestPoint(QuestPoint);
        changePackedScene(sceneName);
        QuestPoint questPoint = (QuestPoint)InThisSceneQuestScene.Instantiate();
        questPoint.Position = GetRandomQuestPoint(QuestPoint);
        activeQuestPoints.Add(questPoint);
        GetTree().Root.AddChild(questPoint);

        
    
    }


    public void changePackedScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Forrest":
                InThisSceneQuestScene = forrestQuestScene;
                break;
            case "Cave":
                InThisSceneQuestScene = caveQuestScene;
                break;
            case "Village":
                InThisSceneQuestScene = villageQuestScene;
                break;
            default:
                break;
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
