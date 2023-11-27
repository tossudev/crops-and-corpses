using Godot;
using System;
using System.Collections.Generic;

public partial class Quest : Node
{
    public string QuestName { get; private set; }

    public int difficulty { get; private set; }
    public int startDay { get; private set; }

    public string Description { get; private set; }
    
    public string SceneName { get; private set; }
    public List<string> Stages { get; private set; }
    public Scene.RootScene Location { get; }
   
    public Vector2 Position { get; internal set; }


    public Quest(string questName, int difficulty, int startDay, QuestType type, Scene.RootScene location)
    {
        QuestName = questName;
        SetDesc(type, difficulty, location);
        this.startDay = startDay;
        SetStages(type);
        
        Location = location;
    }

    void SetDesc(QuestType type, int difficulty, Scene.RootScene location)
    {
        switch (type)
        {
            case QuestType.Rescue:

                string plural = difficulty > 1 ? "s" : "";
                Description = $"Rescue {difficulty} villager{plural} from {location.Name}.";
                break;
            
            case QuestType.BridgeBuild:
                //TODO?
                Description = "";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    void SetStages(QuestType type)
    {
        switch (type)
        {
            case QuestType.Rescue:

                Stages = new List<string> { "Find", "Rescue", "Deliver" };
                break;
            
            case QuestType.BridgeBuild:
                //TODO?
                Description = "";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    public bool IsQuestComplete()
    {
        return Stages.Count == 0;
    }

    public void CompleteQuestStage(string stage)
    {
        if (Stages.Contains(stage))
        {
            Stages.Remove(stage);
        }
    }

    public string GetQuestStage()
    {
        return Stages[0];
    }

    public string GetQuestName()
    {
        return QuestName;
    }

    public string GetQuestDescription()
    {
        return Description;
    }

    public Scene.RootScene GetQuestLocation()
    {
        return Location;
    }

    public List<string> GetQuestStages()
    {
        return Stages;
    }
}

public enum QuestType
{
    Rescue,
    BridgeBuild
}