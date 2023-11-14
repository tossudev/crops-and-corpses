namespace QuestNamespace
{
    public class Quest
    {
        public string Title { get; private set; }
        public string Description { get; private set;}

        public int Difficulty { get; private set; }
        public bool IsCompleted { get; private set; }

        public QuestType Type { get; private set; }

        public Quest(string questTitle, string newQuestDescription, QuestType type, int difficulty)
        {
            Difficulty = difficulty;
            Title = questTitle;
            Description = newQuestDescription;
            Type = type;
            IsQuestCompleted = false;
        }

       

        public bool IsQuestCompleted { get; private set; }

        public enum QuestType
        {
            RescueMission,
            CollectMission,
        }

        public void CompleteQuest()
        {
            IsQuestCompleted = true;
        }
    }
}
