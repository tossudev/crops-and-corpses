using Godot;
using System;

public partial class QuestBoardUi : Control
{

const string MenuButtonPath = "%DiffcultyButton";
	MenuButton QuestButton;

	const string BUTTON_FOREST_NODENAME =  "%ForestButton";
	Button _forestButton;

	const string BUTTON_RUINS_NODENAME = "%RuinsButton";
	Button _ruinsButton;

	const string BUTTON_CAVE_NODENAME = "%CaveButton";
	Button _caveButton;


	const string button_Dif1 = "%dif1";
	Button Dif1Button;

	const string button_Dif2 = "%dif2";
	Button Dif2Button;
	
	const string button_Dif3 = "%dif3";
	Button Dif3Button;

	const string BUTTON_CLOSE_NODENAME = "%MainCloseButtonContainer/CloseButton";
	Button _closeButton;

	const string label_CDiff = "%Cdif";
	Label CDiffLabel;

	QuestManager questManager;
	int _selectedDiff;

	GlobalTime _globalTime;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CloseQuestBoard();
		_globalTime = GetNodeOrNull<GlobalTime>("/root/GlobalTime");
		questManager = GetNode<QuestManager>("/root/QuestManager");

		_ruinsButton.Visible = false;
		_caveButton.Visible = false;

		_closeButton = GetNode<Button>(BUTTON_CLOSE_NODENAME);
		
		_forestButton = GetNode<Button>(BUTTON_FOREST_NODENAME);
		_ruinsButton = GetNode<Button>(BUTTON_RUINS_NODENAME);
		_caveButton = GetNode<Button>(BUTTON_CAVE_NODENAME);
		Dif1Button = GetNode<Button>(button_Dif1);
		Dif2Button = GetNode<Button>(button_Dif2);
		Dif3Button = GetNode<Button>(button_Dif3);
		CDiffLabel = GetNode<Label>(label_CDiff);
		
		
		// Button mapping
		_forestButton.Pressed += () => questManager.StartRescueQuest(Scene.Forest, _selectedDiff);
		_ruinsButton.Pressed += () => questManager.StartRescueQuest(Scene.Ruins, _selectedDiff);
		_caveButton.Pressed += () => questManager.StartRescueQuest(Scene.Cave, _selectedDiff);

		Dif1Button.Pressed += () => SetQuestDifficulty(1);
		Dif2Button.Pressed += () => SetQuestDifficulty(2);
		Dif3Button.Pressed += () => SetQuestDifficulty(3);
		
		_closeButton.Pressed += CloseQuestBoard;
	}
	


	void CheckIfQuestStartedToday()
	{
		if (questManager.GetActiveQuest().startDay == _globalTime.GetDay())
		{
			// TODO: disable other ui. Display text that quest was started today
		}
	}
	
	void SetQuestDifficulty(int diff)
	{
		_selectedDiff = diff;
		CDiffLabel.Text = diff.ToString();
	
	}
void SetLevelsActive()
{
	var rawTownStats = GetNode<RawTownStats>("/root/RawTownStats");
	_forestButton.Disabled = false;
	if (rawTownStats.isRuinsUnlocked)
	{
		_ruinsButton.Visible = true;
	   
	}
	if (rawTownStats.isMineshaftUnlocked)
	{
		_caveButton.Visible = true;
	  
	}
} 



	// close the quest board
	void CloseQuestBoard()
	{
		Visible = false;
		
	}

	// open the quest board
	public void OpenQuestBoard()
	{
		Visible = true;
		SetLevelsActive();
		
	}
}
