using Godot;
using System.Collections.Generic;

public partial class QuestManager : Node
{
    GlobalTime globalTime;

    public int SelectedDifficulty = 1;
    private Quest activeQuest;

    public override void _Ready()
    {
        globalTime = GetNodeOrNull<GlobalTime>("/root/GlobalTime");
        if (globalTime == null)
        {
            GD.PrintErr("GlobalTime not found or not initialized.");
            // Handle the error as needed, e.g., return or throw an exception.
            return;
        }
    }


    public void StartQuest(string questName, string questDescription, List<string> questStages, string questLocation)
    {
        int StartDay = globalTime.GetDay();
        GD.Print($"Start day: {StartDay}");

        if (activeQuest == null && StartDay < globalTime.GetDay())
        {
            Quest newQuest = new Quest
            {
                Name = questName,

                Description = questDescription,

                Stages = questStages,

                Location = questLocation
            };
            SetActiveQuest(newQuest);
        }
        else
        {
            GD.Print("Quest already active");
        }
    }

    public void StartForestQuest()
    {
        List<string> questStages = new List<string> { "Find", "Rescue", "Deliver" };
        StartQuest("Forest Quest", "Rescue Villager From Forest", questStages, "Forest");
        GD.Print("Forest Quest Started");
    }

    public void StartRuinsQuest()
    {
        List<string> questStages = new List<string> { "Find", "Rescue", "Deliver" };
        StartQuest("Ruins Quest", "Rescue Villager From Ruins", questStages, "Ruins");
        GD.Print("Ruins Quest Started");
    }

    public void StartCaveQuest()
    {
        List<string> questStages = new List<string> { "Find", "Rescue", "Deliver" };
        StartQuest("Cave Quest", "Rescue Villager From cave", questStages, "Cave");
        GD.Print("Cave Quest Started");
    }

    public static void LoadQuest()
    {
        // Your implementation here
    }


    public Quest GetActiveQuest() => activeQuest;

    public void SetActiveQuest(Quest quest) => activeQuest = quest;


    public void CompleteQuestStage(string stage) => activeQuest?.CompleteQuestStage(stage);


    internal object GetQuestName()
    {
        return activeQuest?.Name;
    }
}