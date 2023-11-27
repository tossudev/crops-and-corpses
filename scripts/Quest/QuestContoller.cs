
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

            public void ActivateQuestPointForCurrentScene(string sceneName)
            {
                // If the current quest location is the same as the current scene, activate the quest point for that scene
                if(questManager.GetActiveQuest().Location == sceneName)
                {
                questPointManager.ActivateQuestPointForScene(sceneName);
                }
            }

}

    