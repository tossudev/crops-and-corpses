using Godot;
using System;
using System.Collections.Generic;

public enum QuestStatus
{
    Inactive,
    Active,
    Completed
}

public partial class Quest : Node
{
    public string QuestTitle { get; set; }
    public string QuestDescription { get; set; }
    public QuestStatus Status { get; set; } = QuestStatus.Inactive;
    public List<string> Stages { get; set; } = new List<string>();
    public int CurrentStage { get; set; } = 0;

    public void Initialize(string title, string description, List<string> stages, string sceneName)
    {
        QuestTitle = title;
        QuestDescription = description;
        Stages = stages;
        Status = QuestStatus.Inactive;
        CurrentStage = 0;
        SceneName = sceneName;
    }

    public string SceneName { get; private set; }
    public Vector2 Position { get; internal set; }

    public void StartQuest()
    {
        if (Status == QuestStatus.Inactive)
        {
            Status = QuestStatus.Active;
        }
    }

    public void CompleteStage()
    {
        if (Status == QuestStatus.Active && CurrentStage < Stages.Count - 1)
        {
            CurrentStage++;
            GD.Print($"Stage {CurrentStage + 1} completed.\nNext Stage: {Stages[CurrentStage]}\n---");
        }
        else if (Status == QuestStatus.Active && CurrentStage == Stages.Count - 1)
        {
            CompleteQuest();
        }
    }

    public void CompleteQuest()
    {
        if (Status == QuestStatus.Active)
        {
            Status = QuestStatus.Completed;
           
        }
    }

    public void ResetQuest()
    {
        Status = QuestStatus.Inactive;
        CurrentStage = 0;
    }


}
