using Godot;
using System;

public partial class QuesBoard : Control
{
	const string UI_OPEN_QUEST_BOARD_NODENAME = "%res://scenes/Quest/QuestBoardUi.tscn";
	QuestBoardUi _questBoardUi;
	
	const string BUTTON_OPEN_QUEST_BOARD_NODENAME = "%res://scenes/Quest/QuestBoardUi.tscn";
	Button _openQuestBoardButton;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_questBoardUi = GetNode<QuestBoardUi>(UI_OPEN_QUEST_BOARD_NODENAME);

		_openQuestBoardButton = GetNode<Button>(BUTTON_OPEN_QUEST_BOARD_NODENAME);

		_openQuestBoardButton.Pressed += () => _questBoardUi.OpenQuestBoard();
	}

	

	
}
