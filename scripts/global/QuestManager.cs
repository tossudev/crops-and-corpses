using Godot;
using System;
using System.Collections.Generic;

public partial class QuestManager : Node
{
	GlobalTime globalTime;
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


	async void StartQuest(int difficulty, QuestType type, Scene.RootScene Location)
	{
		int StartDay = globalTime.GetDay();
		GD.Print($"Start day: {StartDay}");


		if (await PlayerInfo.GetActiveQuest() == null)
		{

			Quest newQuest = new Quest(difficulty, StartDay, type, Location);

			PlayerInfo.SetActiveQuest(newQuest);
		}
		else
		{
			GD.Print("Quest already active");
		}
	}


	public async void StartRescueQuest(Scene.RootScene location, int difficulty)
	{
		if (difficulty <= 0) return;

		if (await PlayerInfo.GetActiveQuest() != null) return;

		for (int i = 0; i < difficulty; i++)
		{
			VillagerManager.villagerManagerInstance.AddNewVillagerRawData();
		}

		StartQuest(difficulty, QuestType.Rescue, location);


		GD.Print($"Rescue Quest started at {location.Name} with difficulty {difficulty}");
	}


	public async void FinishQuest()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			GD.PushError("Can't finish a null quest");
			return;
		}

		TownManager.GainExp(quest.questDifficulty switch
		{
			1 => ExpGain.BIG,
			2 => ExpGain.VERY_BIG,
			3 => ExpGain.HUGE,
			_ => ExpGain.MEDIUM
		});

		PlayerInfo.SetActiveQuest(null);
	}

	public async void StartTutorialQuest()
	{
		if (await PlayerInfo.GetActiveQuest() != null) return;

		StartQuest(0, QuestType.Tutorial, null);
	}


	public async void FinishTutorialQuest()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			GD.PushError("Can't finish a null quest");
			return;
		}

		TownManager.GainExp(ExpGain.SMALL);

		PlayerInfo.SetActiveQuest(null);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed("<toggle_inventory>"))
		{
			inventoryOpen();

		}

		if (@event.IsActionPressed("<toggle_crafting>"))
		{
			CraftingOpen();

		}

		if (@event.IsActionPressed("<toggle_quest_journal>"))
		{
			QuestJournalOpen();

		}
	}



	public async void inventoryOpen()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			GD.PushError("Can't finish a null quest");
			return;
		}

		if (quest.type == QuestType.Tutorial)
		{
			if (quest.stages.Contains(QuestStage.OpenInventory))
			{
				quest.CompleteQuestStage(QuestStage.OpenInventory);
				quest.ChangeQuestDescription("Press 'C' to open crafting menu.");

			}
		}
	}

	public async void CraftingOpen()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			GD.PushError("Can't finish a null quest");
			return;
		}

		if (quest.type == QuestType.Tutorial)
		{
			{
				if (quest.stages.Contains(QuestStage.OpenInventory)) return;
				quest.CompleteQuestStage(QuestStage.OpenCrafting);
				quest.ChangeQuestDescription("Press 'J' to open quest journal.");

			}
		}
	}

	public async void QuestJournalOpen()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			GD.PushError("Can't finish a null quest");
			return;
		}

		if (quest.type == QuestType.Tutorial)
		{

			if (quest.stages.Contains(QuestStage.OpenCrafting)) return;
			if (quest.stages.Contains(QuestStage.OpenQuestJournal))
			{
				quest.CompleteQuestStage(QuestStage.OpenQuestJournal);
				quest.ChangeQuestDescription("Click on the town hall to finish the tutorial.");

			}
		}


	}


	public async void townHallClicked()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			GD.PushError("Can't finish a null quest");
			return;
		}

		if (quest.type == QuestType.Tutorial)


		{
			if (quest.stages.Contains(QuestStage.OpenQuestJournal)) return;
			if (quest.stages.Contains(QuestStage.ClickOnTownHall))
			{
				quest.CompleteQuestStage(QuestStage.ClickOnTownHall);
				FinishQuest();

			}
		}
	}






}
