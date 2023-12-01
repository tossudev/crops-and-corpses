using Godot;
using System;
using System.Collections.Generic;

public partial class QuestPointManager : Node
{
	
    private List<QuestPoint> activeQuestPoints = new List<QuestPoint>();
    private QuestManager questManager;

    PackedScene forestQuestScene = (PackedScene)GD.Load("res://scenes/quest/forest_quest_scene.tscn");
    PackedScene caveQuestScene = (PackedScene)GD.Load("res://scenes/quest/cave_quest_scene.tscn");
    PackedScene ruinsQuestScene = (PackedScene)GD.Load("res://scenes/quest/ruins_quest_scene.tscn");

    PackedScene InThisSceneQuestScene;

    [Export]
    public Node2D[] QuestPoint;


    public override void _Ready()
    {
        base._Ready();

        

     
      

        questManager = GetNode<QuestManager>("/root/QuestManager");

        // If the current quest location is the same as the current scene, activate the quest point for that scene
        if (SceneManager.IsCurrentScene(this, questManager.GetActiveQuest().Location))
        {
          
            GetRandomQuestPoint(QuestPoint);
            //ActivateQuestPointForScene(SceneManager.GetCurrentScene(this));
        }
    }

    

  // lets chose a random quest point from the array and use the  bool from that questpoint

  public void GetRandomQuestPoint(Node2D[] activeQuestPoints)
  {
       Random random = new Random();
    int randomQuestPointIndex = random.Next(activeQuestPoints.Length);
    Vector2 randomQuestPoint = QuestPoint[randomQuestPointIndex].Position;

    // Check if the QuestPoint node exists before trying to get it
    if (QuestPoint[randomQuestPointIndex].HasNode("QuestPoint"))
    {
        QuestPoint[randomQuestPointIndex].GetNode<QuestPoint>("QuestPoint").isQuestPointActive = true;
        GD.Print(QuestPoint[randomQuestPointIndex].GetNode<QuestPoint>("QuestPoint").isQuestPointActive);
    }
    else
    {
        GD.Print("QuestPoint node does not exist");
        // Handle the case when the QuestPoint node does not exist
        // You can throw an exception or log a message, depending on your needs
    }
  }

  




    

/*
    public Vector2 GetRandomQuestPoint(Node2D[] QuestPoint)
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

    // Example method

   /* public Vector2 GetRandomQuestPoint(Node2D[] QuestPoint)
    {
        Random random = new Random();
        int randomQuestPointIndex = random.Next(QuestPoint.Length);
        Vector2 randomQuestPoint = QuestPoint[randomQuestPointIndex].Position;
        return randomQuestPoint;
    }
   
    public void ActivateQuestPointForScene(Scene.RootScene scene)
    {
        GetRandomQuestPoint(QuestPoint);
        changePackedScene(scene);
        QuestPoint questPoint = (QuestPoint)InThisSceneQuestScene.Instantiate();
        questPoint.Position = GetRandomQuestPoint(QuestPoint);
        activeQuestPoints.Add(questPoint);
        GetTree().Root.AddChild(questPoint);

        
    
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

*/
}
