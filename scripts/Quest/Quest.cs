using Godot;
using System;
using System.Collections.Generic;

public partial class Quest : Node
{
	 public string QuestName { get; set; }
    public string Description { get; set; }
    public string SceneName { get; set; }
    public List<string> Stages { get; set; }
    public string Location { get; set; }
    public Vector2 Position { get; internal set; }


    public void Initialize(string questName, string description, List<string> stages,  string location = "")
    {
        QuestName = questName;
        Description = description;
        Stages = stages;
        Location = location;
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

    public string GetQuestLocation()
    {
        return Location;
    }

    public List<string> GetQuestStages()
    {
        return Stages;
    }




  


}
