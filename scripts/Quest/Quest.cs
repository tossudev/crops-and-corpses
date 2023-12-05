using Godot;
using System;
using System.ComponentModel;
using Godot.Collections;

public partial class Quest : Node
{
    // Keys
    public const string QUEST_DIFFICULTY_KEY = "difficulty";
    public const string QUEST_START_DAY_KEY = "startDay";
    public const string QUEST_DESCRIPTION_KEY = "description";
    public const string QUEST_TYPE_KEY = "questType";
    public const string QUEST_STAGES_KEY = "stages";
    public const string QUEST_LOCATION_KEY = "location";
    
    
    public int questDifficulty { get; private set; }
    public int startDay { get; private set; }
    public string description { get; private set; }

    public QuestType type;
    
    public Array<string> stages { get; private set; }
    public Scene.RootScene location { get; private set; }

    public Quest () {}
    
    public Quest(int difficulty, int startDay, QuestType type, Scene.RootScene location)
    {
        SetDesc(type, difficulty, location);
        this.startDay = startDay;
        SetStages(type);
        
        this.location = location;
    }

    void SetDesc(QuestType type, int difficulty, Scene.RootScene location)
    {
        switch (type)
        {
            case QuestType.Rescue:

                string plural = difficulty > 1 ? "s" : "";
                description = $"Rescue {difficulty} villager{plural} from {location.Name}.";
                questDifficulty = difficulty;
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

                stages = new Array<string> { "Find", "Rescue", "Deliver" };
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    public bool IsQuestComplete()
    {
        return stages.Count == 0;
    }

    public void CompleteQuestStage(string stage)
    {
        if (stages.Contains(stage))
        {
            stages.Remove(stage);
        }
    }

    public string GetQuestStage()
    {
        return stages[0];
    }

    public string GetQuestDescription()
    {
        return description;
    }

    public string ChangeQuestDescription(string description)
    {
        return this.description = description;
    }

    
    public int GetStartDay()
    {
        return startDay;
    }
    
    public static Dictionary GetDictionary(Quest quest)
    {
        Variant questStages = quest.stages;
        
        Dictionary questData = new Dictionary
        {
            { QUEST_DESCRIPTION_KEY, quest.description },
            { QUEST_DIFFICULTY_KEY, quest.questDifficulty },
            { QUEST_LOCATION_KEY, quest.location.ToString() },
            { QUEST_TYPE_KEY, (int) quest.type },
            { QUEST_START_DAY_KEY, quest.startDay },
            { QUEST_STAGES_KEY, questStages }
        };

        return questData;
    }

    public static Quest LoadQuestFromData(Dictionary questDictionary)
    {
        if (questDictionary == null)
        {
            GD.PrintErr("Save data is null, can't load player info!");
            return null;
        }

        Quest loadedQuest = new Quest
        {
            questDifficulty = (int) questDictionary[QUEST_DIFFICULTY_KEY],
            startDay = (int) questDictionary[QUEST_START_DAY_KEY],
            description = (string) questDictionary[QUEST_DESCRIPTION_KEY],
            type = (QuestType) (int) questDictionary[QUEST_TYPE_KEY],
            stages = (Array<string>) questDictionary[QUEST_STAGES_KEY],
            location = Scene.GetRootSceneByName((string) questDictionary[QUEST_LOCATION_KEY])
        };

        return loadedQuest;
    }
    
}

   

public enum QuestType
{
    Rescue,
    BridgeBuild
}
