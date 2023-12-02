using Godot;
using System;

public partial class QuestJournal : Control
{
	QuestManager questManager;

	const string TEXTLABEL_QUESTTEXT = "%QuestText";
	Label QuestTextLabel;


	public override void _Ready()
	{
		questManager = GetNode<QuestManager>("/root/QuestManager");
		QuestTextLabel = GetNode<Label>(TEXTLABEL_QUESTTEXT);
		QuestTextLabel.Visible = true;
		UpdateQuestJournal();


	}

	public void UpdateQuestJournal()
	{
		if (questManager.GetActiveQuest != null)
		{
			QuestTextLabel.Text = questManager.GetActiveQuest().GetQuestDescription().ToString();

	
		}
		else
		{
			QuestTextLabel.Text = "Go to the quest board to start a quest.";
		}
	}

	void toggleQuestJournal()
	{
		UpdateQuestJournal();
		if (Visible == true)
		{
			QuestTextLabel.Visible = false;
		}
		else
		{
			QuestTextLabel.Visible = true;
		}
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if (@event.IsActionPressed("Toggel_QuestJournal"))
		{
			toggleQuestJournal();
			
			
		}
    }


}
