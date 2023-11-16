using Godot;

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
    [Export] int _totalExperience;
    public int totalExperience => _totalExperience;
    
    [Export] int _townHallLevel;
    public int townHallLevel => _townHallLevel;

    [Export] int _populationCap;
    public int populationCap => _populationCap;
    
    
    // Unlocks

    [Export] bool _isRuinsUnlocked;                 // Bridge is built
    public bool isRuinsUnlocked => _isRuinsUnlocked;
    
    [Export] bool _isMineshaftUnlocked;             // Mineshaft entrance is cleared
    public bool isMineshaftUnlocked => _isMineshaftUnlocked;
    
    [Export] bool _isCaveStalagmiteMined;           // Stalagmite is mined from the mineshaft side of the cave
    public bool isCaveStalagmiteMined => _isCaveStalagmiteMined;
    
    
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
    
    public RawTownStats TownStatsAsRaw()
    {
        return new RawTownStats(
            totalExperience: _totalExperience,
            townHallLevel: _townHallLevel,
            populationCap: _populationCap,
            
            
            
            isRuinsUnlocked: _isRuinsUnlocked,
            isMineshaftUnlocked: _isMineshaftUnlocked,
            isCaveStalagmiteMined: _isCaveStalagmiteMined,
            
            
            
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