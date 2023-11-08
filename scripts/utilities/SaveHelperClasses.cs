using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class RawTownStats : GodotObject
{
    // Town stats
    public int totalExperience;
    public const string TOTAL_EXPERIENCE_KEY = "totalExperience";
    
    public int townHallLevel;
    public const string TOWN_HALL_LEVEL_KEY = "townHallLevel";

    public int populationCap;
    public const string POPULATION_CAP_KEY = "populationCap";
    
    
    
    // Soldier stats
    public int soldierAttackSpeed;
    public const string SOLDIER_ATTACK_SPEED_KEY = "soldierAttackSpeed";

    public int soldierAccuracy;
    public const string SOLDIER_ACCURACY_KEY = "soldierAccuracy";
    
    
    
    // Farmer stats
    public int farmerWalkSpeed;
    public const string FARMER_WALK_SPEED_KEY = "farmerWalkSpeed";

    public int farmerMaxFarms;
    public const string FARMER_MAX_FARMS_KEY = "farmerMaxFarms";

    public int pesticideEffectiveness;
    public const string PESTICIDE_EFFECTIVENESS_KEY = "pesticideEffectiveness";
    
    // Walls
    public int wallHP;
    public const string WALL_HP_KEY = "wallHP";

    public bool spikyWalls;
    public const string SPIKY_WALLS_KEY = "spikyWalls";

    // Houses
    public int houseHP;
    public const string HOUSE_HP_KEY = "houseHP";
    
    public RawTownStats () {}

    public RawTownStats(int totalExperience,int townHallLevel, int populationCap,
        int soldierAttackSpeed, int soldierAccuracy,
        int farmerWalkSpeed, int farmerMaxFarms, int pesticideEffectiveness,
        int wallHP, bool spikyWalls,
        int houseHP)
    {
        this.totalExperience = totalExperience;
        this.townHallLevel = townHallLevel;
        this.populationCap = populationCap;
        this.soldierAttackSpeed = soldierAttackSpeed;
        this.soldierAccuracy = soldierAccuracy;
        this.farmerWalkSpeed = farmerWalkSpeed;
        this.farmerMaxFarms = farmerMaxFarms;
        this.pesticideEffectiveness = pesticideEffectiveness;
        this.wallHP = wallHP;
        this.spikyWalls = spikyWalls;
        this.houseHP = houseHP;
    }
    
    /// <summary>
    /// Reads TownStats from save data
    /// </summary>
    /// <param name="saveData"></param>
    public static async Task<bool> AssignStatsDataFromDictionary(Dictionary saveData, bool sync = true)
    {
        if (saveData == null) return false;
        
        var townStatsData = saveData[SaveData.TOWN_STATS_KEY];

        
        Dictionary rawStatsDict = (Dictionary) townStatsData;

        SaveData.townHallStats = new RawTownStats(
            totalExperience: (int)rawStatsDict[TOTAL_EXPERIENCE_KEY],
            townHallLevel: (int)rawStatsDict[TOWN_HALL_LEVEL_KEY],
            populationCap: (int)rawStatsDict[POPULATION_CAP_KEY],
            soldierAttackSpeed: (int)rawStatsDict[SOLDIER_ATTACK_SPEED_KEY],
            soldierAccuracy: (int)rawStatsDict[SOLDIER_ACCURACY_KEY],
            farmerWalkSpeed: (int)rawStatsDict[FARMER_WALK_SPEED_KEY],
            farmerMaxFarms: (int)rawStatsDict[FARMER_MAX_FARMS_KEY],
            pesticideEffectiveness: (int)rawStatsDict[PESTICIDE_EFFECTIVENESS_KEY],
            wallHP: (int)rawStatsDict[WALL_HP_KEY],
            spikyWalls: (bool)rawStatsDict[SPIKY_WALLS_KEY],
            houseHP: (int)rawStatsDict[HOUSE_HP_KEY]
        );

        foreach (var kvp in (Dictionary) saveData[SaveData.APPLIED_TOWN_STATS_KEY])
        {
            var appliedUpgrade = new TownUpgrade((int) kvp.Key, (string) kvp.Value);
            
            SaveData.appliedUpgrades.Add(appliedUpgrade);
        }

        if (sync) await SaveData.SyncTownStats();
        return true;
    }
}

[System.Serializable]
public partial class RawInventoryItem : GodotObject
{
    // ITEM KEYS
    public const string ITEM_ID_KEY = "id";
    public const string ITEM_NAME_KEY = "name";
    public const string ITEM_QUANTITY_KEY = "quantity";
    public const string ITEM_STACKSIZE_KEY = "stackSize";
    public const string ITEM_ORGANIZED_INDEX_KEY = "indexInOrganizedInventory";
    
    public int id;
    public string name;
    public int quantity;
    public int stackSize;
    public int indexInOrganizedInventory;

    public RawInventoryItem(int id, string name, int quantity, int stackSize, int indexInOrganizedInventory = -1)
    {
        this.id = id;
        this.name = name;
        this.quantity = quantity;
        this.stackSize = stackSize;
        this.indexInOrganizedInventory = indexInOrganizedInventory;
    }

    public int SpaceRemainingInStack => stackSize - quantity;

    public bool isValidIndex => (indexInOrganizedInventory >= 0 &&
                                 indexInOrganizedInventory < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE);
    
    
    /// <summary>
    /// Reads inventory data from save data
    /// </summary>
    /// <param name="saveData"></param>
    public static async Task ReadInventoryDataFromFile(Dictionary saveData, bool sync = true)
    {
        SaveData.organizedPlayerInventory.Clear();
        
        if (SaveData.organizedPlayerInventory.Count < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE)
        {
            // Init inventory array with null values
            for (int i = 0; i < PlayerInventoryData.PLAYER_INVENTORY_MAX_SIZE; i++)
            {
                SaveData.organizedPlayerInventory.Add(null);
            }
        }

        if (saveData != null)
        {
            Array organizedInventoryItemData = (Array) saveData[SaveData.ORGANIZED_INVENTORY_ITEMS_KEY];
            await Task.Run(() =>
            {
                foreach (var rawItemVariant in organizedInventoryItemData)
                {
                    Dictionary itemDataDict = (Dictionary)rawItemVariant; 
                
                    RawInventoryItem convertedRawItem = new RawInventoryItem(
                        (int) itemDataDict[ITEM_ID_KEY],
                        (string) itemDataDict[ITEM_NAME_KEY],
                        (int) itemDataDict[ITEM_QUANTITY_KEY],
                        (int) itemDataDict[ITEM_STACKSIZE_KEY],
                        (int) itemDataDict[ITEM_ORGANIZED_INDEX_KEY]);
                
                    SaveData.organizedPlayerInventory[convertedRawItem.indexInOrganizedInventory] = convertedRawItem;
                }

            });
        }
        
        await SaveData.SyncInventory(sync);
    }
}

[System.Serializable]
public partial class RawSaveData : GodotObject
{
    public RawTownStats townStats = new ();
    public List<TownUpgrade> appliedUpgrades = new ();
    public List<RawInventoryItem> inventoryItems = new ();
    public Array<RawInventoryItem> organizedInventoryItems = new ();
    
    public Dictionary GetFullDataDictionary()
    {
        Dictionary fullDictionary = new();
        
        
        // Town stats
        fullDictionary.Add(SaveData.TOWN_STATS_KEY,new Dictionary()
        {
            { RawTownStats.TOTAL_EXPERIENCE_KEY, townStats.totalExperience },
            { RawTownStats.TOWN_HALL_LEVEL_KEY, townStats.townHallLevel },
            { RawTownStats.POPULATION_CAP_KEY, townStats.populationCap },
            { RawTownStats.SOLDIER_ATTACK_SPEED_KEY, townStats.soldierAttackSpeed },
            { RawTownStats.SOLDIER_ACCURACY_KEY, townStats.soldierAccuracy },
            { RawTownStats.FARMER_WALK_SPEED_KEY, townStats.farmerWalkSpeed },
            { RawTownStats.FARMER_MAX_FARMS_KEY, townStats.farmerMaxFarms },
            { RawTownStats.PESTICIDE_EFFECTIVENESS_KEY, townStats.pesticideEffectiveness },
            { RawTownStats.WALL_HP_KEY, townStats.wallHP },
            { RawTownStats.SPIKY_WALLS_KEY, townStats.spikyWalls },
            { RawTownStats.HOUSE_HP_KEY, townStats.houseHP }
        });

        Dictionary appliedUpgradeDict = new ();
        
        foreach (var appliedUpgrade in appliedUpgrades)
        {
            appliedUpgradeDict.Add(appliedUpgrade.id, appliedUpgrade.upgradeHeader);
        }
        fullDictionary.Add(SaveData.APPLIED_TOWN_STATS_KEY, appliedUpgradeDict);
        
        
        
        // All items in inventory
        Dictionary inventoryItemsDict = new();
        inventoryItems.ForEach(item =>
        {
            inventoryItemsDict.Add(item.id, new Dictionary()
            {
                { RawInventoryItem.ITEM_NAME_KEY, item.name },
                { RawInventoryItem.ITEM_QUANTITY_KEY, item.quantity },
                { RawInventoryItem.ITEM_STACKSIZE_KEY, item.stackSize}
            });
        });
        fullDictionary.Add(SaveData.INVENTORY_ITEMS_KEY,inventoryItemsDict);

        
        
        // Organized inventory
        Array organizedInventoryItemsDictArray = new();

        foreach (var rawInventoryItem in organizedInventoryItems)
        {
            if (rawInventoryItem == null) continue;
            
            organizedInventoryItemsDictArray.Add(new Dictionary()
            {
                { RawInventoryItem.ITEM_ID_KEY, rawInventoryItem.id },
                { RawInventoryItem.ITEM_NAME_KEY, rawInventoryItem.name },
                { RawInventoryItem.ITEM_QUANTITY_KEY, rawInventoryItem.quantity },
                { RawInventoryItem.ITEM_STACKSIZE_KEY, rawInventoryItem.stackSize},
                { RawInventoryItem.ITEM_ORGANIZED_INDEX_KEY, rawInventoryItem.indexInOrganizedInventory}
            });
        }
        fullDictionary.Add(SaveData.ORGANIZED_INVENTORY_ITEMS_KEY, organizedInventoryItemsDictArray);

        
        return fullDictionary;
    }
}