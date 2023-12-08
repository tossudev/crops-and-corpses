using Godot;
using System;

public partial class VillagerDropOff : Node2D
{
    public async void _on_drop_off_area_body_entered(Node body)
    {
        if (body is Villager villager)
        {
            if (!SceneManager.IsCurrentScene(this, Scene.Town))
            {
                var quest = await PlayerInfo.GetActiveQuest();
                var questManager = GetNode<QuestManager>("/root/QuestManager");
                if ((!quest?.CompleteQuestStage(QuestStage.Deliver)) ?? false) return;
                questManager.FinishQuest();
                
                villager.QueueFree();
                GD.Print("Villager Drop Off");
            }
        }
    }
}