
using Godot;

using System;
using System.Collections.Generic;

public partial class QuestPointManager : Node
{
    private List<QuestPoint> questPoints = new List<QuestPoint>();
    private Quest activeQuest = null;

    public void AddQuest(Quest quest)
    {
        activeQuest = quest;
    }

    public Vector2 GetRandomQuestPoint()
    {
        if (questPoints.Count > 0 && activeQuest != null)
        {
            // Pick a random quest point from the list
            int randomIndex = GD.RandRange(0, questPoints.Count);
            return questPoints[randomIndex].GlobalPosition;
        }

        return Vector2.Zero;
    }

    public void InstantiateQuestAtPoint(PackedScene questScene, Vector2 point)
    {
        Quest newQuest = (Quest)questScene.Instantiate();
        newQuest.Position = point;
        GetTree().CurrentScene.AddChild(newQuest);

        GD.Print($"Quest instantiated at point: {point}");

        // Assign the quest to the QuestPoint
        QuestPoint questPoint = newQuest.GetNode<QuestPoint>(".");
        questPoint.AssignQuest(activeQuest);
    }
}
