using Godot;
using System;

public partial class QuestJournal : Control
{
	QuestManager questManager;

	const string TEXTLABEL_QUESTTEXT = "%QuestText";
	Label QuestTextLabel;
	
	const string DISTANCE_QUESTTEXT = "%QuestDistance";
	static Label DistanceTextLabel;


	public override void _Ready()
	{
		questManager = GetNode<QuestManager>("/root/QuestManager");
		QuestTextLabel = GetNode<Label>(TEXTLABEL_QUESTTEXT);
		QuestTextLabel.Visible = true;
		
		DistanceTextLabel = GetNode<Label>(DISTANCE_QUESTTEXT);
		
		
		UpdateQuestJournal();
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (TownManager.EveryXSecond(3))
        {
	        UpdateQuestJournal();
        }
    }
	

	public async void UpdateQuestJournal()
	{
		var quest = await PlayerInfo.GetActiveQuest();

		if (quest != null)
		{
			QuestTextLabel.Text = quest.GetQuestDescription();
		}
		else
		{
			QuestTextLabel.Text = SceneManager.IsCurrentScene(this, Scene.Town)
				? "Open quest journal to start a new quest"
				: "Go to town for a new quest";
			
			UpdateDistanceText(0);
		}
	}
	
	public static void UpdateDistanceText(int distance)
	{
		if (DistanceTextLabel == null) return;
		DistanceTextLabel.Visible = distance != 0; 
		
		DistanceTextLabel.Text = $"Distance to mission area : {distance}m";
	}
}
