using System.Threading.Tasks;
using Godot;
using Godot.Collections;

[GlobalClass, Icon("res://icon.svg")]
public partial class TownStats : Resource
{
    public enum ExpGain {
        
        VerySmall = 10,
        Small = 25,
        Medium = 50,
        Big = 75,
        Large = 125,
        Huge = 200
    }
    
    // Town stats
    [Export] float _totalExperience;
    public float totalExperience => _totalExperience;
    
    [Export] int _townHallLevel;
    public int townHallLevel => _townHallLevel;

    [Export] int _populationCap;
    public int populationCap => _populationCap;
    
    
    // Soldier stats
    [Export] int _soldierAttackSpeed;
    public int soldierAttackSpeed => _soldierAttackSpeed;

    [Export] int _soldierAccuracy;
    public int soldierAccuracy => _soldierAccuracy;

    
    
    // Farmer stats
    [Export] int _farmerWalkSpeed;
    public int farmerWalkSpeed => _farmerWalkSpeed;

    [Export] int _farmerMaxFarms;
    public int farmerMaxFarms => _farmerMaxFarms;

    [Export] int _pesticideEffectiveness;
    public int pesticideEffectiveness => _pesticideEffectiveness;

    // Walls
    [Export] int _wallHP;
    public int wallHP => _wallHP;

    [Export] bool _spikyWalls;
    public bool spikyWalls => _spikyWalls;

    // Houses
    [Export] int _houseHP;
    public int houseHP => _houseHP;
    
    
    /// <summary>
    /// Reads TownStats from save data
    /// </summary>
    /// <param name="saveData"></param>
    public static bool ReadStatsDataFromFile(Dictionary saveData)
    {
        if (saveData == null) return false;
        
        var townStatsData = saveData[SaveData.TOWN_STATS_KEY];

        
        Dictionary rawStatsDict = (Dictionary) townStatsData;

        SaveData.townHallStats = new RawTownStats(
            totalExperience: (float)rawStatsDict[RawTownStats.TOTAL_EXPERIENCE_KEY],
            townHallLevel: (int)rawStatsDict[RawTownStats.TOWN_HALL_LEVEL_KEY],
            populationCap: (int)rawStatsDict[RawTownStats.POPULATION_CAP_KEY],
            soldierAttackSpeed: (int)rawStatsDict[RawTownStats.SOLDIER_ATTACK_SPEED_KEY],
            soldierAccuracy: (int)rawStatsDict[RawTownStats.SOLDIER_ACCURACY_KEY],
            farmerWalkSpeed: (int)rawStatsDict[RawTownStats.FARMER_WALK_SPEED_KEY],
            farmerMaxFarms: (int)rawStatsDict[RawTownStats.FARMER_MAX_FARMS_KEY],
            pesticideEffectiveness: (int)rawStatsDict[RawTownStats.PESTICIDE_EFFECTIVENESS_KEY],
            wallHP: (int)rawStatsDict[RawTownStats.WALL_HP_KEY],
            spikyWalls: (bool)rawStatsDict[RawTownStats.SPIKY_WALLS_KEY],
            houseHP: (int)rawStatsDict[RawTownStats.HOUSE_HP_KEY]
        );
        return true;
    }
    
    public RawTownStats TownStatsAsRaw()
    {
        return new RawTownStats(
            totalExperience: _totalExperience,
            townHallLevel: _townHallLevel,
            populationCap: _populationCap,
            soldierAttackSpeed: _soldierAttackSpeed,
            soldierAccuracy: _soldierAccuracy,
            farmerWalkSpeed: _farmerWalkSpeed,
            farmerMaxFarms: _farmerMaxFarms,
            pesticideEffectiveness: _pesticideEffectiveness,
            wallHP: _wallHP,
            spikyWalls: _spikyWalls,
            houseHP: _houseHP);
    }
}