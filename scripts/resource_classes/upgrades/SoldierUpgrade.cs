using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class SoldierUpgrade : TownUpgrade
{
    [Export] int _soldierAttackSpeed;
    public int soldierAttackSpeed => _soldierAttackSpeed;

    [Export] int _soldierAccuracy;
    public int soldierAccuracy => _soldierAccuracy;
    
    public override string GetEffectDescription()
    {
        string description = "";

        if (soldierAttackSpeed > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            int percentage = GetPercentage(_soldierAttackSpeed, TownManager.currentTownStats.soldierAttackSpeed);
            description += "Archer tower attack SPD +" + percentage + "%";
        }

        if (soldierAccuracy > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            int percentage = GetPercentage(_soldierAccuracy, TownManager.currentTownStats.soldierAccuracy);
            description += "Archer tower accuracy +" + percentage + "%";
        }
        
        return description;
    }
}