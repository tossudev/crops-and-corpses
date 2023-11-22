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
    public const string VILLAGER_CURRENT_OCCUPATION_KEY = "currentOccupation";
    public const string VILLAGER_CURRENT_STATE_KEY = "currentState";
    
    public int id;
    public string name;
    public string lore;
    public bool isTownPopulation;
    public VillagerOccupation currentOccupation;
    public VillagerState currentState;
    public int xCoord;
    public int yCoord;
    //TODO

    public VillagerRawData() {}

    public VillagerRawData(string name, string lore, bool isTownPopulation)
    {
        id = SaveData.allVillagers.Count;
        this.name = name;
        this.lore = lore;
        this.isTownPopulation = isTownPopulation;
        currentOccupation = VillagerOccupation.Builder;
        currentState = VillagerState.ChooseTask;
    }

    public VillagerRawData(
        int id,
        string name,
        string lore,
        bool isTownPopulation,
        VillagerOccupation currentOccupation,
        VillagerState currentState)
    {
        this.id = id;
        this.name = name;
        this.lore = lore;
        this.isTownPopulation = isTownPopulation;
        this.currentOccupation = currentOccupation;
        this.currentState = currentState;
    }
    
    
    /// <summary>
    /// Reads all villager data from save data
    /// </summary>
    /// <param name="saveData"></param>
    public static async Task ReadVillagerDataFromFile(Dictionary saveData, bool spawnAll = true)
    {
        SaveData.allVillagers.Clear();

        if (saveData != null)
        {
            Array rawVillagerVariants = (Array) saveData[SaveData.VILLAGER_DATA_KEY];
            await Task.Run(() =>
            {
                foreach (var rawVillagerVariant in rawVillagerVariants)
                {
                    Dictionary villagerDataDict = (Dictionary) rawVillagerVariant; 
                
                    VillagerRawData convertedRawVillager = new VillagerRawData(
                        (int) villagerDataDict[VILLAGER_ID_KEY],
                        (string) villagerDataDict[VILLAGER_NAME_KEY],
                        (string) villagerDataDict[VILLAGER_LORE_KEY],
                        (bool) villagerDataDict[VILLAGER_IS_TOWN_POPULATION_KEY],
                        (VillagerOccupation) (int) villagerDataDict[VILLAGER_CURRENT_OCCUPATION_KEY],
                        (VillagerState) (int) villagerDataDict[VILLAGER_CURRENT_STATE_KEY]);
                
                    SaveData.allVillagers.Add(convertedRawVillager);
                }
            });
        }
        
        // TODO: spawn all town villagers
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
                { VILLAGER_CURRENT_OCCUPATION_KEY, (int) villager.currentOccupation },
                { VILLAGER_CURRENT_STATE_KEY, (int) villager.currentState }
            });
        });

        return villagerDataDict;
    }
}