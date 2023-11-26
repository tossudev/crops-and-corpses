using System.Collections.Generic;
using Godot;



public partial class Quest : Node2D
{
   
    public string QuestName { get; set; }
    public string Description { get; set; }
    
    public string SceneName { get; set; }
    public List<string> Stages { get; set; }
    
    public int Difficulty { get; set; }
    public string Location { get; internal set; }

    public void Initialize(string questName, string description, List<string> stages, string sceneName, int difficulty = 1, string location = "")
    {
        QuestName = questName;
        Description = description;
        Stages = stages;
        SceneName = sceneName;
        Difficulty = difficulty;
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

   



    public void PrintQuestInfo()
    {
        GD.Print($"Quest name: {QuestName}");
        GD.Print($"Quest description: {Description}");
        GD.Print($"Quest stages: {string.Join(", ", Stages)}");
        GD.Print($"Quest scene: {SceneName}");
    }

    public void PrintQuestStatus()
    {
        if (IsQuestComplete())
        {
            GD.Print("Quest complete!");
        }
        else
        {
            GD.Print($"Quest incomplete. Remaining stages: {string.Join(", ", Stages)}");
        }
    }



}



