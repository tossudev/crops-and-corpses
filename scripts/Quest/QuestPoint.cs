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


    const string Node2D_QuestZombieSpawn = "%QuestZombieSpawn";
    private Node2D spawnZombiePoint;


    const string Node2D_VillagerPoint = "%VillagerSpawn";
    private Node2D villagerSpawnPoint;
    private PackedScene zombieScene;
    private PackedScene villagerScene;
    
    private SpawnScript zombieSpawn;
    PlayerController playerController;
    private List<Node> zombiesInArea = new List<Node>();


    private int zombieAmount = 4;
    private int villagerAmount = 1;
    public bool isQuestPointActive = false;

    int playerDistanceToQuestPoint;
    int SpawnRange = 300;

    int villagerSpawnRange = 50;

    int CurrentDifficulty;


    bool isZombiesSpawned = false;


    public override void _Ready()
    {
        base._Ready();


        questManager = GetNode<QuestManager>("/root/QuestManager");

        zombieScene = (PackedScene)ResourceLoader.Load("res://scenes/zombie/Zombie.tscn");
        spawnZombiePoint = GetNode<Node2D>(Node2D_QuestZombieSpawn);
        villagerSpawnPoint = GetNode<Node2D>(Node2D_VillagerPoint);
        playerController = (PlayerController)GetTree().GetFirstNodeInGroup("player");
        zombieSpawn = GetParent().GetParent().GetNodeOrNull<SpawnScript>("ZombieSpawn");
    }


    Quest _activeQuest;
    Timer _questFetchTimer;


    async void QuestFetchTimerTimeout()
    {
        _questFetchTimer.Paused = true;
        _questFetchTimer.QueueFree();
        _questFetchTimer = null;

        _activeQuest = await PlayerInfo.GetActiveQuest();
    }
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (!isQuestPointActive) return;
        
        if (_activeQuest == null)
        {
            if (_questFetchTimer == null)
            {
                _questFetchTimer = new Timer()
                {
                    WaitTime = GD.RandRange(2f, 4f),
                    Autostart = true
                };

                _questFetchTimer.Timeout += QuestFetchTimerTimeout;
                AddChild(_questFetchTimer);
            }

            return;
        }
        
        
        
        playerDistanceToQuestPoint = (int) playerController.GlobalPosition.DistanceTo(GlobalPosition);

        if (_activeQuest.HasStage(QuestStage.Find))
        {
            QuestJournal.UpdateDistanceText(playerDistanceToQuestPoint);
        }
        
        if (_activeQuest.HasStage(QuestStage.Kill))
        {
            if (playerDistanceToQuestPoint <= SpawnRange)
            {
                _activeQuest.CompleteQuestStage(QuestStage.Find);
                KillStage();
            }
        }
    }


    void SpawnVillagers()
    {
        GD.Print("SpawnVillagers");
        VillagerManager.villagerManagerInstance.SpawnQuestVillagers(villagerSpawnPoint.GlobalPosition);
    }

    

    public void SpawnZombies()
    {
        Vector2 offsetVector = new Vector2(GD.Randi() % 3, GD.Randi() % 3);


        if (zombieSpawn != null && isZombiesSpawned == false)
        {
            for (int i = 0; i < GetSpawnedZombieAmount(); i++)
            {
                zombieSpawn.SpawnZombieAtPoint(spawnZombiePoint.GlobalPosition + offsetVector);
            }

            _activeQuest?.ChangeQuestDescription("Clear the area of zombies");
            isZombiesSpawned = true;
        }
        else
        {
            GD.PrintErr("ZombieSpawn not found or not initialized.");
        }
    }


    int GetSpawnedZombieAmount()
    {
        CurrentDifficulty = _activeQuest?.questDifficulty ?? 0;

        return CurrentDifficulty switch
        {
            1 => 4,
            2 => 6,
            3 => 8,
            _ => 4,
        };
    }

    void KillStage()
    {
        _activeQuest.ChangeQuestDescription("Kill all zombies");

        if (!isZombiesSpawned)
        {
            SpawnZombies();
            SpawnVillagers();
        }

        int zombiesRemaining = zombieSpawn.GetZombieQuestListCount();
        
        if (zombiesRemaining == 0)
        {
            if (_activeQuest.CompleteQuestStage(QuestStage.Kill))
            {
                _activeQuest.ChangeQuestDescription("Talk to the villagers");
            }
        }
        
        QuestJournal.UpdateDistanceText(0);
    }


    public void ActivateQuestPoint()
    {
        isQuestPointActive = true;
    }
}