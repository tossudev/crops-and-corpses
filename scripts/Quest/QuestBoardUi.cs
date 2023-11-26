using Godot;
using Godot.NativeInterop;
using System;

public partial class QuestBoardUi : Panel
{

	CheckButton DifficultyButton1;
	const string DifficultyButton1_BUTTON_NODENAME  = "Dif1";

	CheckButton DifficultyButton2;
	const string DifficultyButton2_BUTTON_NODENAME  = "Dif2";

	CheckButton DifficultyButton3;
	const string DifficultyButton3_BUTTON_NODENAME = "Dif3";

	Button QuestButton1;
	const string QuestButton1_BUTTON_NODENAME = "ForrestLevel";
	Button QuestButton2;
	const string QuestButton2_BUTTON_NODENAME = "RuinsLevel";
	Button QuestButton3;
	const string QuestButton3_BUTTON_NODENAME = "CaveLevel";

	ButtonGroup DifficultyButtonGroup;

	QuestManager questManager;
	TownManager townManager;
	VillagerManager villagerManager;

	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		DifficultyButton1 = GetNode<CheckButton>(DifficultyButton1_BUTTON_NODENAME);
		DifficultyButton2 = GetNode<CheckButton>(DifficultyButton2_BUTTON_NODENAME);
		DifficultyButton3 = GetNode<CheckButton>(DifficultyButton3_BUTTON_NODENAME);

		QuestButton1 = GetNode<Button>(QuestButton1_BUTTON_NODENAME);
		QuestButton2 = GetNode<Button>(QuestButton2_BUTTON_NODENAME);
		QuestButton3 = GetNode<Button>(QuestButton3_BUTTON_NODENAME);

		DifficultyButtonGroup = GetNode<ButtonGroup>("DifficultyButtonGroup");

		
		


		questManager = GetNode<QuestManager>("/root/QuestManager");
		VillagerManager villagerManager = GetNode<VillagerManager>("/root/VillagerManager");
	}


private void OnDifficultyButton3Toggled(bool buttonPressed)
    {
		questManager.SetDifficulty(3);
		
		
		
    }

    private void OnDifficultyButton2Toggled(bool buttonPressed)
    {
		questManager.SetDifficulty(2);
		
	}

    private void OnDifficultyButton1Toggled(bool buttonPressed)
    {
		questManager.SetDifficulty(1);
	
		

    }

	private void OnQuestButton1Pressed()
	{
		if (questManager.GetActiveQuest() != null)
		{
			GD.Print("Quest already active");
			
			return;
		}else
		{
			questManager.StartForrestQuest();
			GD.Print("Forrest Quest Started");
		}

		
		
	
	}
	

	private void OnQuestButton2Pressed()
	{
		if (questManager.GetActiveQuest() != null)
		{
			GD.Print("Quest already active");
			SetvillagerstoQuest();
			return;
		}else
		{
			questManager.StartRuinsQuest();
			SetvillagerstoQuest();
			GD.Print("Ruins Quest Started");
		}
	}

	private void OnQuestButton3Pressed()
	{
		if (questManager.GetActiveQuest() != null)
		{
			GD.Print("Quest already active");
			return;}
		else
		{
			questManager.StartCaveQuest();
			SetvillagerstoQuest();
			GD.Print("Cave Quest Started");
		}
	}



	public void SetvillagerstoQuest()
	{
		

		if ((int)questManager.GetDifficulty() == 1)
		{
			villagerManager.AddNewVillagerRawData();
		}
		else if ((int)questManager.GetDifficulty() == 2)
		{
			villagerManager.AddNewVillagerRawData();
			villagerManager.AddNewVillagerRawData();
		}
		else if ((int)questManager.GetDifficulty() == 3)
		{
			villagerManager.AddNewVillagerRawData();
			villagerManager.AddNewVillagerRawData();
			villagerManager.AddNewVillagerRawData();
		}
		else
		{
			GD.Print("No Quest Active");
		}
		
	}



}
