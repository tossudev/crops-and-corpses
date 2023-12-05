
using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class FarmerUpgrade : TownUpgrade
{
    [Export] int _farmerWalkSpeed;
    public int farmerWalkSpeed => _farmerWalkSpeed;
    
    public override string GetEffectDescription()
    {
        string description = "";

        if (farmerWalkSpeed > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            int percentage = GetPercentage(farmerWalkSpeed, TownManager.currentTownStats.farmerWalkSpeed);
            description += "Farmer walk speed +" + percentage + "%";
        }
        
        return description;
    }
}