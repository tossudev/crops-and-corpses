using Godot;
using System;

public partial class QuestBoardUi : Node2D
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

	QuestManager questManager;

	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		DifficultyButton1 = GetNode<CheckButton>(DifficultyButton1_BUTTON_NODENAME);
		DifficultyButton2 = GetNode<CheckButton>(DifficultyButton2_BUTTON_NODENAME);
		DifficultyButton3 = GetNode<CheckButton>(DifficultyButton3_BUTTON_NODENAME);

		QuestButton1 = GetNode<Button>(QuestButton1_BUTTON_NODENAME);
		QuestButton2 = GetNode<Button>(QuestButton2_BUTTON_NODENAME);
		QuestButton3 = GetNode<Button>(QuestButton3_BUTTON_NODENAME);


		questManager = GetNode<QuestManager>("/root/QuestManager");
	}
		

	public void OnQuestButtonPressed1()
	{
		questManager.StartForrestQuest();
		GD.Print(questManager.GetActiveQuest().Name);

	}


	public void OnQuestButtonPressed2()
	{
		questManager.StartRuinsQuest();
		GD.Print(questManager.GetActiveQuest().Name);
	}

	public void OnQuestButtonPressed3()
	{
		questManager.StartCaveQuest();
		GD.Print(questManager.GetActiveQuest().Name);
	}

	public void OnDifficultyButtonToggled1()
	{
		
	}

	


	// button set difficulty 
	public void OnDifficultyButtonPressed1()
	{
		questManager.SetDifficulty(1);
		GD.Print(questManager.GetActiveQuest().Difficulty);
	}

	public void OnDifficultyButtonPressed2()
	{
		questManager.SetDifficulty(2);
			GD.Print(questManager.GetActiveQuest().Difficulty);
	}

	public void OnDifficultyButtonPressed3()
	{
		questManager.SetDifficulty(3);
		GD.Print(questManager.GetActiveQuest().Difficulty);
	}



}
