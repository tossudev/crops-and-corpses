using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

public partial class QuestPoint : Node2D
{
    private QuestManager questManager;

    VillagerManager villagerManager;
    private QuestController questController;
    private ZombieManager zombieManager;
    private Node2D spawnZombiePoint;

    private Node2D villagerSpawnPoint;
    private PackedScene zombieScene;
    private PackedScene villagerScene;
    private Queue<CharacterBody2D> zombieQueue = new Queue<CharacterBody2D>();

    private int zombieAmount = 4;
    private int villagerAmount = 1;
    private bool isQuestPointActive = false;

    int CurrentDifficulty;



    public override void _Ready()
    {
        questManager = GetNode<QuestManager>("/root/QuestManager");
        questController = GetNode<QuestController>("/root/QuestController");
        zombieManager = GetNode<ZombieManager>("/root/ZombieManager");
        spawnZombiePoint = GetNode<Node2D>("SpawnZombiePoint");
        zombieScene = (PackedScene)GD.Load("res://LilianTests/Prefabs/zombie_with_hitbox.tscn");
        villagerScene = (PackedScene)GD.Load("res://scenes/villager/villager.tscn");
        villagerManager = GetNode<VillagerManager>("/root/VillagerManager");

        CurrentDifficulty = questManager.GetActiveQuest().Difficulty;
    }

    public override void _Process(double delta)
    {
        if (isQuestPointActive)
        {
            SetZombieAmount(questManager.GetActiveQuest().Difficulty);
            SpawnZombies();
        }

        if (zombieQueue.Count == 0)
        {
            isQuestPointActive = false;
            SpawnVillager();
            questManager.CompleteQuestStage(questManager.GetActiveQuest().Stages[0]);
            // move to the next stage of the quest
        }
    }

    public void SpawnVillager()
    {
        if (isQuestPointActive)
        {
           GD.Print("villagerSpawnes");
           
        }
    }

    public void SpawnZombies()
    {
        if (isQuestPointActive)
        {
            for (int i = 0; i < zombieAmount; i++)
            {
                CharacterBody2D zombie = (CharacterBody2D)zombieScene.Instantiate();
                zombie.Position = spawnZombiePoint.Position;
                GetTree().CurrentScene.AddChild(zombie);
                zombieQueue.Enqueue(zombie);
            }
        }
    }

    public void SetZombieAmount(int CurrentDifficulty)
    {
        switch (CurrentDifficulty)
        {
            case 1:
                zombieAmount = 3;
                break;
            case 2:
                zombieAmount = 5;
                break;
            case 3:
                zombieAmount = 7;
                break;
            default:
                zombieAmount = 4;
                break;
        }
    }

    public void SetVillagerAmount(int CurrentDifficulty)
    {
        switch (CurrentDifficulty)
        {
            case 1:
                villagerAmount = 1;
                break;
            case 2:
                villagerAmount = 2;
                break;
            case 3:
                villagerAmount = 3;
                break;
            default:
                villagerAmount = 5;
                break;
        }
    }
    
        

    public void ActivateQuestPoint()
    {
        isQuestPointActive = true;
    }
}
