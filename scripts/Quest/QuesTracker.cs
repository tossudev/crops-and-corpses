using Godot;
using System;
using System.Linq;

public partial class QuesTracker : Node
{
	private Node2D[] ForrestQuestPoints;
	private Node2D[] ForrestQuestPoints2;
	private Node2D[] ForrestQuestPoints3;
	private Node2D[] RuinsQuestPoints;
	private Node2D[] CaveQuestPoints;

	private QuestManager questManager;

	private PackedScene ForrestQuestScene;
	private PackedScene RuinsQuestScene;
	private PackedScene CaveQuestScene;

	private Random random = new Random();

	public override void _Ready()
	{
		questManager = GetNode<QuestManager>("/root/QuestManager");
		ForrestQuestScene = GD.Load<PackedScene>("res://scenes/Quests/ForrestQuest.tscn");
		RuinsQuestScene = GD.Load<PackedScene>("res://scenes/Quests/RuinsQuest.tscn");
		CaveQuestScene = GD.Load<PackedScene>("res://scenes/Quests/CaveQuest.tscn");
		// if quest is active, spawn quest
		// if quest is not active, do nothing

		if(questManager.GetCurrentQuest() != null)
		{
			SpawnQuest();
		}
		else
		{
			GD.Print("No quest active");
		}
	}

	public void SpawnQuest()
	{
		switch (questManager.CurrentDifficulty)
		{
			case 1:
			case 2:
				SpawnForrestQuest();
				break;
			case 3:
				SpawnRuinsQuest();
				break;
			case 4:
				SpawnCaveQuest();
				break;
			default:
				GD.Print("No quest found");
				break;
		}
	}

	private void SpawnCaveQuest()
	{
		if (CaveQuestPoints.Length == 0)
		{
			GD.Print("No quest points found");
			return;
		}

		int randomIndex = random.Next(CaveQuestPoints.Length);
		var quest = (Node2D)CaveQuestScene.Instantiate();
		quest.Position = CaveQuestPoints[randomIndex].Position;
		questManager.AddChild(quest);
	}

	private void SpawnRuinsQuest()
	{
		if (RuinsQuestPoints.Length == 0)
		{
			GD.Print("No quest points found");
			return;
		}

		int randomIndex = random.Next(RuinsQuestPoints.Length);
		var quest = (Node2D)RuinsQuestScene.Instantiate();
		quest.Position = RuinsQuestPoints[randomIndex].Position;
		questManager.AddChild(quest);
	}

	private void SpawnForrestQuest()
	{
		Node2D[] questPoints = ForrestQuestPoints;
		if (questManager.CurrentDifficulty == 2)
		{
			questPoints = questPoints.Union(ForrestQuestPoints2).ToArray();
		}

		if (questPoints.Length == 0)
		{
			GD.Print("No quest points found");
			return;
		}

		int randomIndex = random.Next(questPoints.Length);
		var quest = (Node2D)ForrestQuestScene.Instantiate();
		quest.Position = questPoints[randomIndex].Position;
		questManager.AddChild(quest);
	}
}