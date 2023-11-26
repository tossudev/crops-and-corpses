using Godot;
using System;

public partial class QuestJournal : Node2D
{

	QuestManager questManager;


	
	const string TextEditPath = "QuestJournalText";
	TextEdit questJournalText;

	bool questJournalOpen = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	
		UpdateQuestJournal();
		questManager = GetNode<QuestManager>("/root/QuestManager");
		questJournalText = GetNodeOrNull<TextEdit>(TextEditPath);
		

	}

	
	public override void _Process(double delta)
	{
		UpdateQuestJournal();

		
		
	}

	public void UpdateQuestJournal()
	{
		if(questManager.GetActiveQuest() == null)
		{
			questJournalText.Text = "No Active Quest";

			return;

		}
		questJournalText = GetNode<TextEdit>("QuestJournalText");
		questJournalText.Text = questManager.GetActiveQuest().Description;
	}

	public void _on_QuestJournalButton_pressed()
	{
		if (questJournalOpen == false)
		{
			questJournalOpen = true;
			questJournalText.Visible = true;
		}
		else
		{
			questJournalOpen = false;
			questJournalText.Visible = false;
		}
	}
 
}
