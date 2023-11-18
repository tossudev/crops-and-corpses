using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class QuestPoint
    {
         private Quest assignedQuest;

    public Vector2 GlobalPosition { get; internal set; }

    public void AssignQuest(Quest quest)
    {
        assignedQuest = quest;
    }

    public void Interact()
    {
        if (assignedQuest != null)
        {
            // Simulate player interacting with the quest point
            GD.Print($"Interacting with quest point. Current Stage: {assignedQuest.CurrentStage + 1}/{assignedQuest.Stages.Count}");
            assignedQuest.CompleteStage();
        
    }
    }
}