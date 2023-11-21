using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Controls the quests in the game.
/// </summary>
public partial class QuestController : Node2D
{
    private QuestManager questManager;
    private QuestPointManager questPointManager;

    public override void _Ready()
    {
        questManager = GetNode<QuestManager>("/root/QuestManager");
        questPointManager = GetNode<QuestPointManager>("/root/QuestPointManager");
    }

    /// <summary>
    /// Activates the quest point for the current scene.
    /// </summary>
    /// <param name="sceneName">The name of the current scene.</param>
    public void ActivateQuestPointForCurrentScene(string sceneName)
    {
        questPointManager.ActivateQuestPointForScene(sceneName);
    }

    /// <summary>
    /// Initializes a quest from the quest board.
    /// </summary>
    /// <param name="sceneName">The name of the current scene.</param>
    public void InitializeQuestFromQuestBoard(string sceneName)
    {
      
    }

}
    