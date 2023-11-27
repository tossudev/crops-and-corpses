using Godot;
using System;

public partial class QuestBoardUi : Control
{

const string MenuButtonPath = "%DiffcultyButton";
	MenuButton QuestButton;

	const string button_forest =  "%ForestButton";
	Button ForestButton;

	const string button_ruins = "%RuinsButton";
	Button RuinsButton;

	const string button_Cave = "%CaveButton";
	Button CaveButton;


	const string button_Dif1 = "%dif1";
	Button Dif1Button;

	const string button_Dif2 = "%dif2";
	Button Dif2Button;
	
	const string button_Dif3 = "%dif3";
	Button Dif3Button;

	const string label_CDiff = "%Cdif";
	Label CDiffLabel;

	QuestManager questManager;

	VillagerManager villagerManager;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ForestButton = GetNode<Button>(button_forest);
		RuinsButton = GetNode<Button>(button_ruins);
		CaveButton = GetNode<Button>(button_Cave);
		Dif1Button = GetNode<Button>(button_Dif1);
		Dif2Button = GetNode<Button>(button_Dif2);
		Dif3Button = GetNode<Button>(button_Dif3);
		CDiffLabel = GetNode<Label>(label_CDiff);
		
		
		// Button mapping
		ForestButton.Pressed += () => questManager.StartForestQuest();
		RuinsButton.Pressed += () => questManager.StartRuinsQuest();
		CaveButton.Pressed += () => questManager.StartCaveQuest();

		Dif1Button.Pressed += () => SetQuestDifficulty(1);
		Dif2Button.Pressed += () => SetQuestDifficulty(2);
		Dif3Button.Pressed += () => SetQuestDifficulty(3);
	}

	public void _Process(double delta)
	{
		CDiffLabel.Text = questManager.SelectedDifficulty.ToString();
	}
    

	void SetQuestDifficulty(int diff)
	{
		questManager.SelectedDifficulty = diff;
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
