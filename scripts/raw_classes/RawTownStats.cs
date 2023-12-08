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
    
    public int providedHomes;
    public const string PROVIDED_HOMES_KEY = "providedHomes";
    
    // Unlocks 
    public bool isRuinsUnlocked;
    public const string RUINS_UNLOCKED_KEY = "isRuinsUnlocked";

    public bool isMineshaftUnlocked;
    public const string MINESHAFT_UNLOCKED_KEY = "isMineshaftUnlocked";

    public bool isCaveStalagmiteMined;
    public const string CAVE_STALAGMITE_MINED_KEY = "isCaveStalagmiteMined";
    
    
    public bool isDIYBridgeBuilt;
    public const string DIY_BRIDGE_BUILT_KEY = "isDIYBridgeBuilt";
    
    
    // Soldier stats
    public int soldierAttackSpeed;
    public const string SOLDIER_ATTACK_SPEED_KEY = "soldierAttackSpeed";

    public int soldierAccuracy;
    public const string SOLDIER_ACCURACY_KEY = "soldierAccuracy";
    
    
    
    // Farmer stats
    public int farmerWalkSpeed;
    public const string FARMER_WALK_SPEED_KEY = "farmerWalkSpeed";
    
    // Walls
    public int wallHP;
    public const string WALL_HP_KEY = "wallHP";

    // Houses
    public int houseHP;
    public const string HOUSE_HP_KEY = "houseHP";
    
    //NOT SAVED
    public bool gameFinished;
    
    public RawTownStats () {}

    public RawTownStats(int totalExperience,int townHallLevel, int populationCap, int providedHomes,
        bool isRuinsUnlocked, bool isMineshaftUnlocked, bool isCaveStalagmiteMined, bool isDIYBridgeBuilt,
        int soldierAttackSpeed, int soldierAccuracy,
        int farmerWalkSpeed,
        int wallHP,
        int houseHP)
    {
        this.totalExperience = totalExperience;
        this.townHallLevel = townHallLevel;
        this.populationCap = populationCap;
        this.providedHomes = providedHomes;


        this.isRuinsUnlocked = isRuinsUnlocked;
        this.isMineshaftUnlocked = isMineshaftUnlocked;
        this.isCaveStalagmiteMined = isCaveStalagmiteMined;
        this.isDIYBridgeBuilt = isDIYBridgeBuilt;
        
        
        
        this.soldierAttackSpeed = soldierAttackSpeed;
        this.soldierAccuracy = soldierAccuracy;
        
        
        
        this.farmerWalkSpeed = farmerWalkSpeed;
        
        
        
        this.wallHP = wallHP;
        
        
        
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
            providedHomes: (int)rawStatsDict[PROVIDED_HOMES_KEY],
            
            
            
            isRuinsUnlocked: (bool)rawStatsDict[RUINS_UNLOCKED_KEY],
            isMineshaftUnlocked: (bool)rawStatsDict[MINESHAFT_UNLOCKED_KEY],
            isCaveStalagmiteMined: (bool)rawStatsDict[CAVE_STALAGMITE_MINED_KEY],
            isDIYBridgeBuilt: (bool)rawStatsDict[DIY_BRIDGE_BUILT_KEY],
            
            
            soldierAttackSpeed: (int)rawStatsDict[SOLDIER_ATTACK_SPEED_KEY],
            soldierAccuracy: (int)rawStatsDict[SOLDIER_ACCURACY_KEY],
            
            
            
            farmerWalkSpeed: (int)rawStatsDict[FARMER_WALK_SPEED_KEY],
            
            
            
            wallHP: (int)rawStatsDict[WALL_HP_KEY],
            
            
            
            houseHP: (int)rawStatsDict[HOUSE_HP_KEY]
        );

        
        // Upgrades
        foreach (var kvp in (Dictionary) saveData[SaveData.APPLIED_TOWN_UPGRADES_KEY])
        {
            var appliedUpgrade = new TownUpgrade((int) kvp.Key, (string) kvp.Value);
            
            SaveData.appliedUpgrades.Add(appliedUpgrade);
        }
        
        // Unlocks
        foreach (var kvp in (Dictionary) saveData[SaveData.APPLIED_TOWN_UNLOCKS_KEY])
        {
            var appliedUnlock = (TownUnlock) (int) kvp.Key;
            SaveData.appliedUnlocks.Add(appliedUnlock);
        }
        
        // Game completed
        SaveData.townHallStats.gameFinished = SaveData.townHallStats.townHallLevel > 4;

        if (sync) await SaveData.SyncTownStats();
        return true;
    }
    
    
    public static Dictionary GetDictionary(RawTownStats townStats)
    {
        if (townStats != null)
        {
            return new Dictionary()
            {
                { TOTAL_EXPERIENCE_KEY, townStats.totalExperience },
                { TOWN_HALL_LEVEL_KEY, townStats.townHallLevel },
                { POPULATION_CAP_KEY, townStats.populationCap },
                { PROVIDED_HOMES_KEY, townStats.providedHomes },
            
            
                { RUINS_UNLOCKED_KEY, townStats.isRuinsUnlocked },
                { MINESHAFT_UNLOCKED_KEY, townStats.isMineshaftUnlocked },
                { CAVE_STALAGMITE_MINED_KEY, townStats.isCaveStalagmiteMined },
                { DIY_BRIDGE_BUILT_KEY, townStats.isDIYBridgeBuilt },
            
            
                { SOLDIER_ATTACK_SPEED_KEY, townStats.soldierAttackSpeed },
                { SOLDIER_ACCURACY_KEY, townStats.soldierAccuracy },
            
            
                { FARMER_WALK_SPEED_KEY, townStats.farmerWalkSpeed },
            
            
                { WALL_HP_KEY, townStats.wallHP },
            
            
                { HOUSE_HP_KEY, townStats.houseHP }
            };
        }
            
        
        GD.PushWarning("TownStats was null @GetDictionary");
        return new Dictionary();
    }
}