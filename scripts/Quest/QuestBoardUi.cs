using Godot;
using System;

public partial class QuestBoardUi : Control
{

const string MenuButtonPath = "DiffcultyButton";
	MenuButton QuestButton;

	const string button_forrest =  "ForrestButton";
	Button ForrestButton;

	const string button_ruins = "RuinsButton";
	Button RuinsButton;

	const string button_Cave = "CaveButton";
	Button CaveButton;


	const string button_Dif1 = "dif1";
	Button Dif1Button;

	const string button_Dif2 = "dif2";
	Button Dif2Button;
	
	const string button_Dif3 = "dif3";
	Button Dif3Button;

	const string label_CDiff = "Cdif";
	Label CDiffLabel;

	QuestManager questManager;

	VillagerManager villagerManager;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ForrestButton = GetNode<Button>(button_forrest);
		RuinsButton = GetNode<Button>(button_ruins);
		CaveButton = GetNode<Button>(button_Cave);
		Dif1Button = GetNode<Button>(button_Dif1);
		Dif2Button = GetNode<Button>(button_Dif2);
		Dif3Button = GetNode<Button>(button_Dif3);
		CDiffLabel = GetNode<Label>(label_CDiff);
	}

	public void _Process(double delta)
	{
		CDiffLabel.Text = questManager.SelectedDifficulty.ToString();
	}



	

	public void On_forrest_button_pressed()
	{
		questManager.StartForrestQuest();
		GD.Print("Forrest button pressed");
	}
	
	
	public void On_RuinsButton_pressed()
	{
		questManager.StartRuinsQuest();
		GD.Print("Ruins button pressed");
	}

	public void On_CaveButton_pressed()
	{
		questManager.StartCaveQuest();
		GD.Print("Cave button pressed");
	}

	public void On_dif1_pressed()
	{
		questManager.SelectedDifficulty = 1;
		GD.Print("Difficulty 1 button pressed");
	}

	public void On_dif2_pressed()
	{	questManager.SelectedDifficulty = 2;
		GD.Print("Difficulty 2 button pressed");
	}

	public void On_dif3_pressed()
	{	
		questManager.SelectedDifficulty = 3;
		GD.Print("Difficulty 3 button pressed");
	}

	public void addRawVillagers()
	{
		if(questManager.SelectedDifficulty == 1)
		villagerManager.AddNewVillagerRawData();
		else if(questManager.SelectedDifficulty == 2)
		{
		villagerManager.AddNewVillagerRawData();
		villagerManager.AddNewVillagerRawData();
		}
	
		else if(questManager.SelectedDifficulty == 3)
		{
		villagerManager.AddNewVillagerRawData();
		villagerManager.AddNewVillagerRawData();
		villagerManager.AddNewVillagerRawData();
		}
	}

	
}
