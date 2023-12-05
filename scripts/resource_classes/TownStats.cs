using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class TownStats : Resource
{
    
    // Town stats
    [Export] int _totalExperience;
    [Export] int _townHallLevel;
    [Export] int _populationCap;                    // Max amount of population
    [Export] int _providedHomes;                    // Current amount of homes for villagers to live in.
                                                    // (1 home for 1 villager, 3-5 per house)
    
    // Unlocks
    [Export] bool _isDIYBridgeBuilt;           // Big tree has been cut down
    [Export] bool _isRuinsUnlocked;                 // Bridge is built
    [Export] bool _isMineshaftUnlocked;             // Mineshaft entrance is cleared
    [Export] bool _isCaveStalagmiteMined;           // Stalagmite is mined from the mineshaft side of the cave
    
    // Soldier stats
    [Export] int _soldierAttackSpeed;
    [Export] int _soldierAccuracy;
    
    
    // Farmer stats
    [Export] int _farmerWalkSpeed;
    
    // Walls
    [Export] int _wallHP;

    // Houses
    [Export] int _houseHP;
    
    public RawTownStats TownStatsAsRaw()
    {
        return new RawTownStats(
            totalExperience: _totalExperience,
            townHallLevel: _townHallLevel,
            populationCap: _populationCap,
            providedHomes: _providedHomes,
            
            
            isRuinsUnlocked: _isRuinsUnlocked,
            isMineshaftUnlocked: _isMineshaftUnlocked,
            isCaveStalagmiteMined: _isCaveStalagmiteMined,
            isDIYBridgeBuilt: _isDIYBridgeBuilt,
            
            
            
            soldierAttackSpeed: _soldierAttackSpeed,
            soldierAccuracy: _soldierAccuracy,
            
            
            
            farmerWalkSpeed: _farmerWalkSpeed,
            
            
            
            wallHP: _wallHP,
            
            
            
            houseHP: _houseHP);
    }
}

public enum ExpGain {
        
    HARVEST_COMMON = 5,
    HARVEST_RARE = 10,
    HARVEST_LEGENDARY = 50,
        
        
    VERY_SMALL = 10,
    SMALL = 25,
    MEDIUM = 50,
    BIG = 75,
    VERY_BIG = 125,
    HUGE = 200
}