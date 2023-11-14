using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quests
{
	public class Quest
	{
		public enum QuestType
		{
			RescueMission
		}

		public string Title { get; }
		public string Description { get; }
		public QuestType Type { get; }
		public int Difficulty { get; }
		public bool IsCompleted { get; private set; }

		public Quest(string title, string description, QuestType type, int difficulty)
		{
			Title = title;
			Description = description;
			Type = type;
			Difficulty = difficulty;
		}

		public void Complete()
		{
			IsCompleted = true;
		}
	}

	public partial class QuestManager : Node
	{
		private readonly List<Quest> _quests = new List<Quest>();
		private readonly Random _random = new();

		public int CurrentDifficulty = 1;

		public Node2D[] QuestPointsForest;
		public Node2D[] QuestPointsCave;
		public void AddQuest(string title)
		{
			_quests.Add(new Quest(title, "", Quest.QuestType.RescueMission, 0));
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

		public Quest GetCurrentQuest()
		{
			return _quests.FirstOrDefault(quest => !quest.IsCompleted);
		}

		

		public int GetCurrentDifficulty()
		{
			return CurrentDifficulty;
		}

		internal IEnumerable<Quest> GetQuests()
		{
			return _quests.Where(quest => !quest.IsCompleted);
		}
	}
}