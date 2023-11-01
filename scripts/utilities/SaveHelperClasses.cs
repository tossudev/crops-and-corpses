using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class RawTownStats : GodotObject
{
    // Town stats
    [Export] public float totalExperience;
    public const string TOTAL_EXPERIENCE_KEY = "totalExperience";
    
    [Export] public int townHallLevel;
    public const string TOWN_HALL_LEVEL_KEY = "townHallLevel";

    [Export] public int populationCap;
    public const string POPULATION_CAP_KEY = "populationCap";
    
    
    
    // Soldier stats
    [Export] public int soldierAttackSpeed;
    public const string SOLDIER_ATTACK_SPEED_KEY = "soldierAttackSpeed";

    [Export] public int soldierAccuracy;
    public const string SOLDIER_ACCURACY_KEY = "soldierAccuracy";
    
    
    
    // Farmer stats
    [Export] public int farmerWalkSpeed;
    public const string FARMER_WALK_SPEED_KEY = "farmerWalkSpeed";

    [Export] public int farmerMaxFarms;
    public const string FARMER_MAX_FARMS_KEY = "farmerMaxFarms";

    [Export] public int pesticideEffectiveness;
    public const string PESTICIDE_EFFECTIVENESS_KEY = "pesticideEffectiveness";
    
    // Walls
    [Export] public int wallHP;
    public const string WALL_HP_KEY = "wallHP";

    [Export] public bool spikyWalls;
    public const string SPIKY_WALLS_KEY = "spikyWalls";
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
            { RawTownStats.SPIKY_WALLS_KEY, townStats.spikyWalls }
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