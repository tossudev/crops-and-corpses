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

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		UpdateQuestJournal();
    }
	

	public async void UpdateQuestJournal()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		QuestTextLabel.Text = quest != null
			? quest.GetQuestDescription()
			: "Open Quest Journal to start a new quest";
	}
}
