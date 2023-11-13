using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class HousingUpgrade : TownUpgrade
{
    [Export] int _houseHP;
    public int houseHP => _houseHP;
    
    public override string GetEffectDescription()
    {
        string description = "";

        if (houseHP > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            description += "Housing HP +" + houseHP;
        }
        
        return description;
    }
}