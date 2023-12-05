using Godot;
using System;

public partial class VillagerDropOff : Node2D
{

	

	public async void _on_drop_off_area_body_entered(Node body)
	{
		if (body is PlayerController)
        {
            var quest = await PlayerInfo.GetActiveQuest();
			var questManager = GetNode<QuestManager>("/root/QuestManager");
            if (!(SceneManager.GetCurrentScene(this) == Scene.Town))
            {
				if (!quest.CompleteQuestStage("Delliver")) return;
				questManager.FinishQuest();
               
                GD.Print("Villager Drop Off");
            }
            return;
      
	  
	    }

		if(body is Villager villager)
		{
			
			if (!(SceneManager.GetCurrentScene(this) == Scene.Town))
			{
				villager.QueueFree();
				villager.rawData.isTownPopulation = true;
				GD.Print("Villager Drop Off");
			}


			}
	}

}
