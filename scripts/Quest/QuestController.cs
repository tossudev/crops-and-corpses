using Godot;
using System;
using System.Collections.Generic;

public partial class QuestController : Node2D
{
  private QuestManager questManager;
    private QuestPointManager questPointManager;

    public override void _Ready()
    {
        questManager = GetNode<QuestManager>("/root/QuestManager");
        questPointManager = GetNode<QuestPointManager>("/root/QuestPointManager");
    }

    public void SetActiveQuestForScene(string sceneName)
    {
        questManager.SetActiveQuestForScene(sceneName);
    }

    public void ActivateQuestPointForCurrentScene(string sceneName)
    {
        questPointManager.ActivateQuestPointForScene(sceneName);
    }
}
    