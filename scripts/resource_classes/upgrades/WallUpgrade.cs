using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class WallUpgrade : TownUpgrade
{
    [Export] int _wallHP;
    public int wallHP => _wallHP;

    [Export] bool _spikyWalls;
    public bool spikyWalls => _spikyWalls;
    public override string GetEffectDescription()
    {
        string description = "";

        if (wallHP > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            description += "Wall HP +" + wallHP;
        }

        if (!TownManager.currentTownStats.spikyWalls && spikyWalls)
        {
            description = AddDelimiterIfNotEmpty(description);
            description += "Walls deal DMG if attacked";
        }
        
        return description;
    }
}