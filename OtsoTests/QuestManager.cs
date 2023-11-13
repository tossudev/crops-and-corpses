using Godot;
using System;
using System.Collections.Generic;

public partial class QuestManager : Node
{
	private List<Quest> _quests = new List<Quest>();
	private Random random = new Random();

	public Node2D[] QuestPoints;

	



	public void AddQuest(string title, string description)
	{
		
		
	}

	public void RemoveQuest(string title)
	{
		foreach (var quest in _quests)
		{
			if (quest.Title == title)
			{
				_quests.Remove(quest);
				return;
			}
		}
	}
	


	public void CompleteQuest(string title)
	{
		foreach (var quest in _quests)
		{
			if (quest.Title == title)
			{
				quest.Complete();
				return;
				
			}

		}

	}

	

	
	 public void CreateRescueMission(string targetName)
    {
        string title = $"Rescue {targetName}";
        string description = $"Rescue {targetName} from danger!";
        Quest rescueMission = new Quest(title, description, Quest.QuestType.RescueMission);
        _quests.Add(rescueMission);

        GD.Print("Rescue mission created: " + title);
    }

	public void GetRandomQuestPoint(){

		int randomIndex = random.Next(QuestPoints.Length);
		Node2D randomQuestPoint = QuestPoints[randomIndex];
		GD.Print("Random quest point: " + randomQuestPoint.Name);
	}

	

	

	
	
	
	
    internal IEnumerable<Quest> GetQuests()
    {
        return _quests.FindAll(quest => !quest.IsCompleted);
    }

}
