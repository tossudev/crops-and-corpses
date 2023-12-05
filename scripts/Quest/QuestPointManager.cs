using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class QuestPointManager : Node
{
    public List<QuestPoint> activeQuestPoints = new List<QuestPoint>();


    QuestManager questManager;


    PackedScene InThisSceneQuestScene;


    public override async void _Ready()
    {
        base._Ready();

        // Add all quest points to the list of active quest points
        foreach (Node node in GetChildren())
        {
            if (node is QuestPoint questPoint)
            {
                activeQuestPoints.Add(questPoint);
            }
        }

        GD.Print("Active quest points count: " + activeQuestPoints.Count);


        questManager = GetNode<QuestManager>("/root/QuestManager");


        questManager = GetNode<QuestManager>("/root/QuestManager");


        questManager = GetNode<QuestManager>("/root/QuestManager");

        if (questManager != null)
        {
            var activeQuest = await PlayerInfo.GetActiveQuest();
            if (activeQuest != null)
            {
                // If the active quest is not at this location, don't activate a quest point.
                if (!SceneManager.IsCurrentScene(this, activeQuest.location))
                {
                    GD.Print("Quest point not active");
                    return;
                }
                
                // If the active quest is at this location, activate a random quest point.
                GetRandomQuestPoint().isQuestPointActive = true;
            }
            else
            {
                GD.Print("No active quest");
            }
        }
        else
        {
            GD.Print("questManager is null");
        }
    }

    // Check if the quest manager exists.

    public QuestPoint GetRandomQuestPoint()
    {
        if (activeQuestPoints.Count > 0)
        {
            Random random = new Random();
            int index = random.Next(activeQuestPoints.Count);
            QuestPoint randomQuestPoint = activeQuestPoints[index];
            return randomQuestPoint;
        }
        else
        {
            GD.Print("No active quest points");
            return null;
        }
    }


// set bool in questpoint to true


    /* public Vector2 GetRandomQuestPoint(Node2D[] QuestPoint)
     {
         Random random = new Random();
         int randomQuestPointIndex = random.Next(QuestPoint.Length);
         Vector2 randomQuestPoint = QuestPoint[randomQuestPointIndex].Position;
         return randomQuestPoint;
     }

     public void changePackedScene(Scene.RootScene rootScene)
     {
         if (rootScene == Scene.Forest)
         {
             InThisSceneQuestScene = forestQuestScene;
         }
         else if (rootScene == Scene.Ruins)
         {
             InThisSceneQuestScene = ruinsQuestScene;
         }
         else if (rootScene == Scene.Cave)
         {
             InThisSceneQuestScene = caveQuestScene;
         }
     }

     // Example method to deactivate a specific quest point
     public void DeactivateQuestPoint(QuestPoint questPoint)
     {
         // Deactivate or perform other actions related to deactivation
         questPoint.QueueFree(); // Assuming QuestPoint is a Node2D, change as needed
         activeQuestPoints.Remove(questPoint);
     }

     // Example method to deactivate all quest points
     public void DeactivateAllQuestPoints()
     {
         foreach (QuestPoint questPoint in activeQuestPoints)
         {
             questPoint.QueueFree(); // Assuming QuestPoint is a Node2D, change as needed
         }
         activeQuestPoints.Clear();
     }

     // Example method to deactivate all quest points in a specific scene
     public void DeactivateAllQuestPointsInScene(Scene.RootScene scene)
     {
         foreach (QuestPoint questPoint in activeQuestPoints)
         {
             if (SceneManager.IsCurrentScene(questPoint, scene))
             {
                 questPoint.QueueFree(); // Assuming QuestPoint is a Node2D, change as needed
             }
         }
         activeQuestPoints.RemoveAll(questPoint => SceneManager.IsCurrentScene(questPoint, scene));
     }

     */
}