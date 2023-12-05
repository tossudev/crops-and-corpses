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

    const string Area2D_ZombieArea = "%ZombieArea";
    private Area2D ZombieArea;
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
        ZombieArea = GetNode<Area2D>(Area2D_ZombieArea);
        zombieSpawn = GetParent().GetParent().GetNodeOrNull<SpawnScript>("ZombieSpawn");

       

        


       
    }



    
       
      public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (isQuestPointActive == true && questManager.GetActiveQuest().GetQuestStage() == "Find")
        {
            playerDistanceToQuestPoint = (int)playerController.GlobalPosition.DistanceTo(GlobalPosition);

            if (!isZombiesSpawned && playerDistanceToQuestPoint <= SpawnRange)
            {

                questManager.CompleteQuestStage("Find");
            
                KillStage();

    

                

               
            }

            if(zombieSpawn.GetZombieQuestListCount() == 0 && isZombiesSpawned == true)
            {
                questManager.GetActiveQuest().CompleteQuestStage("Kill");
                
               
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
        SpawendZombieAmount();
        Vector2 offsetVector = new Vector2(GD.Randi() % 3, GD.Randi() % 3);
        

        if (zombieSpawn != null && isZombiesSpawned == false)
        {
            for (int i = 0; i < zombieAmount; i++)
            {

                //spawnZombiePoint ofset

               
                zombieSpawn.SpawnZombieAtPoint(spawnZombiePoint.GlobalPosition + offsetVector);
            

                GD.Print("SpawnZombies");
            }

            GD.Print(zombieSpawn.GetZombieQuestListCount()); 
            questManager.GetActiveQuest().ChangeQuestDescription("Clear the area of zombies");
            isZombiesSpawned = true;
        }
        else
        {
            GD.PrintErr("ZombieSpawn not found or not initialized.");
            // Handle the error as needed, e.g., return or throw an exception.
            return;
        }
    }

    

    private void SpawendZombieAmount()
    {
        CurrentDifficulty = questManager.GetActiveQuest().difficulty;
        switch (CurrentDifficulty)
        {
            case 1:
                zombieAmount = 4;
                break;
            case 2:
                zombieAmount = 6;
                break;
            case 3:
                zombieAmount = 8;
                break;
            default:
                zombieAmount = 4;
                break;
        }
    }

    void KillStage()
    {
        questManager.GetActiveQuest().ChangeQuestDescription("Kill all zombies");
        SpawnZombies();
        SpawnVillagers();
        if(zombieSpawn.GetZombieQuestListCount() == 0)
        {
            questManager.GetActiveQuest().CompleteQuestStage("Kill");
            questManager.GetActiveQuest().ChangeQuestDescription("Talk to the villagers");
        }
    }

    

   




    public void ActivateQuestPoint()
    {
        isQuestPointActive = true;
    }
    }
    
    