using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

public partial class QuestPoint: Node2D
    {

        private QuestManager questManager;
        private QuestController questController;

        Node2D SpawnZombiePoint;
        

        private ZombieManager zombieManager;

        private int ZombieAmount = 4;
        private int VillagerAmount = 1;

        private bool isQuestPointActive = false;

        public PackedScene ZombieScene { get; set; }

        public PackedScene VillagerScene { get; set; }

        private List<CharacterBody2D> zombieList = new List<CharacterBody2D>();

        public override void _Ready()
        {
            questManager = GetNode<QuestManager>("/root/QuestManager");
            questController = GetNode<QuestController>("/root/QuestController");
            zombieManager = GetNode<ZombieManager>("/root/ZombieManager");
            SpawnZombiePoint = GetNode<Node2D>("SpawnZombiePoint");
           
            
        }

        public override void _Process(double delta)
        {
            if (isQuestPointActive)
            {
                SetZombieAmount(questManager.GetActiveQuest().Difficulty);
                SpawnZombies();
            
                
            }

            if(zombieList.Count == 0)
            {
                isQuestPointActive = false;
                SpawnVillager();
                questManager.CompleteQuestStage(questManager.GetActiveQuest().Stages[0]);
                // move to the next stage of the quest
            
                
            }
        }


        public void SpawnVillager(){
            if(isQuestPointActive){
                for(int i = 0; i < VillagerAmount; i++){
                   VillagerScene = (PackedScene)GD.Load("res://scenes/villager/villager.tscn");
                    CharacterBody2D villager = (CharacterBody2D)VillagerScene.Instantiate();
                    villager.Position = SpawnZombiePoint.Position;
                    GetTree().CurrentScene.AddChild(villager);
                }
            }

        }


        

        public void SpawnZombies()
        {
            // spawn zombies at the quest point

            if (isQuestPointActive)
            {
                for (int i = 0; i < ZombieAmount; i++)
                {
                    // spawn zombies at the quest point
                    ZombieScene = (PackedScene)GD.Load("res://LilianTests/Prefabs/zombie_with_hitbox.tscn");
                    CharacterBody2D zombie = (CharacterBody2D)ZombieScene.Instantiate();
                    zombie.Position = SpawnZombiePoint.Position;
                    GetTree().CurrentScene.AddChild(zombie);
                    zombieList.Add(zombie);
                }
                    
                    
                    
                 
            }
        }


          public void SetZombieAmount(int amount)
        {
           amount = questManager.GetActiveQuest().Difficulty;
        
        
            // set the amount of zombies to spawn according to the quest difficulty

            switch (amount)
            {
                case 1:
                    ZombieAmount = 3;
                    break;
                case 2:
                    ZombieAmount = 5;
                    break;
                case 3:
                    ZombieAmount = 7;
                    break;
                default:
                    ZombieAmount = 4;
                    break;
            }
        }

        public void SetVillagerAmount(int amount)
        {
            amount = questManager.GetActiveQuest().Difficulty;
        
        
            // set the amount of zombies to spawn according to the quest difficulty

            switch (amount)
            {
                case 1:
                    VillagerAmount = 1;
                    break;
                case 2:
                    VillagerAmount = 2;
                    break;
                case 3:
                    VillagerAmount = 3;
                    break;
                default:
                    VillagerAmount = 5;
                    break;
            }
        }


        public void ActivateQuestPoint()
        {
            isQuestPointActive = true;
        }

    }
