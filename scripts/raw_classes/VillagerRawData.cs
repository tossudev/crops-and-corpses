using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class VillagerRawData : GodotObject
{
    // Data keys
    public const string VILLAGER_ID_KEY = "id";
    public const string VILLAGER_NAME_KEY = "name";
    public const string VILLAGER_LORE_KEY = "lore";
    public const string VILLAGER_IS_TOWN_POPULATION_KEY = "isTownPopulation";
    public const string VILLAGER_TYPE_KEY = "villagerType";
    public const string VILLAGER_CURRENT_OCCUPATION_KEY = "currentOccupation";
    public const string VILLAGER_CURRENT_STATE_KEY = "currentState";
    public const string VILLAGER_X_COORD_KEY = "xCoordinate";
    public const string VILLAGER_Y_COORD_KEY = "yCoordinate";
    
    public int id;
    public string name;
    public string lore;
    public bool isTownPopulation;
    public VillagerType villagerType;
    public VillagerOccupation currentOccupation;
    public VillagerState currentState;
    public int xCoord;
    public int yCoord;

    public VillagerRawData() {}

    public VillagerRawData(string name, string lore, bool isTownPopulation)
    {
        id = SaveData.allVillagerData.Count;
        this.name = name;
        this.lore = lore;
        this.isTownPopulation = isTownPopulation;
        SetType();
        currentOccupation = VillagerOccupation.Builder;
        currentState = VillagerState.ChooseTask;
    }

    public VillagerRawData(
        int id,
        string name,
        string lore,
        bool isTownPopulation,
        VillagerType villagerType,
        VillagerOccupation currentOccupation,
        VillagerState currentState,
        int xCoord,
        int yCoord)
    {
        this.id = id;
        this.name = name;
        this.lore = lore;
        this.isTownPopulation = isTownPopulation;
        this.villagerType = villagerType;
        this.currentOccupation = currentOccupation;
        this.currentState = currentState;
        
        this.xCoord = xCoord;
        this.yCoord = yCoord;
    }

    void SetType()
    {
        villagerType = (VillagerType) (GD.Randi() % Enum.GetValues<VillagerType>().Length);
    }
    
    /// <summary>
    /// Reads all villager data from save data
    /// </summary>
    /// <param name="saveData"></param>
    public static async Task ReadVillagerDataFromFile(Dictionary saveData)
    {
        SaveData.allVillagerData.Clear();

        if (saveData != null)
        {
            Dictionary rawVillagerVariants = (Dictionary) saveData[SaveData.VILLAGER_DATA_KEY];
            await Task.Run(() =>
            {
                foreach (var rawVillagerVariant in rawVillagerVariants)
                {
                    Dictionary villagerDataDict = (Dictionary) rawVillagerVariant.Value; 
                
                    VillagerRawData convertedRawVillager = new VillagerRawData(
                        (int) villagerDataDict[VILLAGER_ID_KEY],
                        (string) villagerDataDict[VILLAGER_NAME_KEY],
                        (string) villagerDataDict[VILLAGER_LORE_KEY],
                        (bool) villagerDataDict[VILLAGER_IS_TOWN_POPULATION_KEY],
                        (VillagerType) (int) villagerDataDict[VILLAGER_TYPE_KEY],
                        (VillagerOccupation) (int) villagerDataDict[VILLAGER_CURRENT_OCCUPATION_KEY],
                        (VillagerState) (int) villagerDataDict[VILLAGER_CURRENT_STATE_KEY],
                        (int) villagerDataDict[VILLAGER_X_COORD_KEY],
                        (int) villagerDataDict[VILLAGER_Y_COORD_KEY]
                        );
                
                    SaveData.allVillagerData.Add(convertedRawVillager);
                }
            });
        }
    }



    public static Dictionary GetDictionary(List<VillagerRawData> allVillagers)
    {
        Dictionary villagerDataDict = new ();
        
        allVillagers.ForEach(villager =>
        {
            villagerDataDict.Add(villager.id, new Dictionary()
            {
                { VILLAGER_ID_KEY, villager.id },
                { VILLAGER_NAME_KEY, villager.name },
                { VILLAGER_LORE_KEY, villager.lore },
                { VILLAGER_IS_TOWN_POPULATION_KEY, villager.isTownPopulation },
                { VILLAGER_TYPE_KEY, (int) villager.villagerType },
                { VILLAGER_CURRENT_OCCUPATION_KEY, (int) villager.currentOccupation },
                { VILLAGER_CURRENT_STATE_KEY, (int) villager.currentState },
                { VILLAGER_X_COORD_KEY, villager.xCoord },
                { VILLAGER_Y_COORD_KEY, villager.yCoord }
            });
        });

        return villagerDataDict;
    }
}