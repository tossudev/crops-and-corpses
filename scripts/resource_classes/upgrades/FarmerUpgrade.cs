
using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class FarmerUpgrade : TownUpgrade
{
    [Export] int _farmerWalkSpeed;
    public int farmerWalkSpeed => _farmerWalkSpeed;

    [Export] int _farmerMaxFarms;
    public int farmerMaxFarms => _farmerMaxFarms;

    [Export] int _pesticideEffectiveness;
    public int pesticideEffectiveness => _pesticideEffectiveness;
    
    public override string GetEffectDescription()
    {
        string description = "";

        if (farmerWalkSpeed > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            int percentage = GetPercentage(farmerWalkSpeed, TownManager.currentTownStats.farmerWalkSpeed);
            description += "Farmer walk speed +" + percentage + "%";
        }

        if (farmerMaxFarms > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            description += "Farmers take care of +" + farmerMaxFarms + " farm" + (farmerMaxFarms > 1 ? "s" : "");
        }

        if (pesticideEffectiveness > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            int percentage = GetPercentage(pesticideEffectiveness, TownManager.currentTownStats.pesticideEffectiveness);
            description += "Pesticide effectiveness +" + percentage + "%";
        }
        
        return description;
    }
}