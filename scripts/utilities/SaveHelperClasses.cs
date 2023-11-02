using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class RawTownStats : GodotObject
{
    // Town stats
    public float totalExperience;
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

    public RawTownStats(float totalExperience,int townHallLevel, int populationCap,
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
}

[System.Serializable]
public partial class RawSaveData : GodotObject
{
    public RawTownStats townStats = new ();
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