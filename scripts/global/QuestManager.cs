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
		
		StartTutorialIfNewGame();
	}

	async void StartTutorialIfNewGame()
	{
		await TaskExtensions.SuspendWhile(() => TownManager.currentTownStats != null, 400);

		if (TownManager.currentTownStats.totalExperience == 0)
		{
			if (globalTime.GetDay() == 0)
			{
				StartTutorialQuest();
			}
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

		if (quest.type == QuestType.Rescue)
		{
			foreach (var villagerdata in SaveData.allVillagerRawData.FindAll(data => !data.isTownPopulation))
			{
				villagerdata.isTownPopulation = true;
				villagerdata.xCoord = 0;
				villagerdata.yCoord = 0;
			}
		}
		
		PlayerInfo.SetActiveQuest(null);
	}

	public void StartTutorialQuest()
	{
		StartQuest(1, QuestType.Tutorial, null);
	}
    

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed("toggle_inventory"))
		{
			inventoryOpen();

		}

		if (@event.IsActionPressed("toggle_crafting_window"))
		{
			CraftingOpen();

		}

		if (@event.IsActionPressed("Toggel_QuestJournal"))
		{
			QuestJournalOpen();

		}

		if (@event.IsActionPressed("open_build_menu"))
		{
			BuildMenuOpen();

		}
	}



	public async void inventoryOpen()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			return;
		}

		if (quest.type == QuestType.Tutorial)
		{
			if (quest.HasStage(QuestStage.OpenInventory))
			{
				quest.CompleteQuestStage(QuestStage.OpenInventory);
				quest.ChangeQuestDescription("Press 'Q' to open crafting menu.");

			}
		}
	}

	public async void CraftingOpen()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			return;
		}

		if (quest.type == QuestType.Tutorial)
		{
			{
				if (quest.HasStage(QuestStage.OpenInventory)) return;
				quest.CompleteQuestStage(QuestStage.OpenCrafting);
				quest.ChangeQuestDescription("open 'V' build menu.");

			}
		}
	}

	public async void QuestJournalOpen()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			return;
		}

		if (quest.type == QuestType.Tutorial)
		{

			if (quest.HasStage(QuestStage.ClickOnTownHall)) return;
			if (quest.HasStage(QuestStage.OpenQuestJournal))
			{
				quest.CompleteQuestStage(QuestStage.OpenQuestJournal);
				FinishQuest();
			}
		}


	}

	public async void BuildMenuOpen()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			return;
		}

		if (quest.type == QuestType.Tutorial)
		{
			if (quest.HasStage(QuestStage.OpenCrafting)) return;
			if (quest.HasStage(QuestStage.OpenBuildMenu))
			{
				quest.CompleteQuestStage(QuestStage.OpenBuildMenu);
				quest.ChangeQuestDescription("click on town hall.");
			}
		}
	}


	public async void TownHallClicked()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest == null)
		{
			return;
		}

		if (quest.type == QuestType.Tutorial)


		{
			if (quest.HasStage(QuestStage.OpenCrafting)) return;
			if (quest.HasStage(QuestStage.ClickOnTownHall))
			{
				quest.CompleteQuestStage(QuestStage.ClickOnTownHall);
				quest.ChangeQuestDescription("press 'J' to open quest journal.");
			}
		}
	}






}
