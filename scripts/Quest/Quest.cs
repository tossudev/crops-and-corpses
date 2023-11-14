using Godot;

public class Quest
{
    public string Title { get; private set; }
    public string Description { get; private set;}

    public int Difficulty { get; private set; }
    public bool IsCompleted { get; private set; }

    

    public QuestType questType { get; private set; }

   

    public Quest(string title, string description, QuestType type, int difficulty)
    {

        Difficulty = difficulty;
        Title = title;
        Description = description;
        questType = type;
        IsCompleted = false;
        

    }

    public void Complete()
    {
        IsCompleted = true;
    }

    public enum QuestType
    {
        RescueMission,
        collectMission,

    }
}
