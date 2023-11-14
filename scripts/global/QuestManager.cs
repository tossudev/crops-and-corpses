using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class QuestManager : Node
{
	private List<Quest> _quests = new List<Quest>();
	private Random random = new Random();

	public int CurrentDifficulty = 1;

	public Node2D[] QuestPointsForest;
	public Node2D[] QuestPointsCave;
	public Node2D[] QuestPointsRuins;

    public object CurrentQuest { get; internal set; }

    public void AddQuest(string title, string description)
	{
		// TODO: Implement this method
	}

	public void RemoveQuest(string title)
	{
		_quests.RemoveAll(quest => quest.Title == title);
	}

	public void CompleteQuest(string title)
	{
		var quest = _quests.FirstOrDefault(q => q.Title == title);
		if (quest != null)
		{
			quest.Complete();
		}
	}

	public void CreateRescueMission(int difficulty)
	{
		string title = $"Rescue Villager";
		string description = $"Rescue Villager from danger!";
		Quest rescueMission = new Quest(title, description, Quest.QuestType.RescueMission, difficulty);
		_quests.Add(rescueMission);
		

		GD.Print("Rescue mission created: " + title);
	}

	// get current quest
	public Quest GetCurrentQuest()
	{
		return _quests.FirstOrDefault(quest => !quest.IsCompleted);
	}

	

	internal IEnumerable<Quest> GetQuests()
	{
		return _quests.Where(quest => !quest.IsCompleted);
	}
}