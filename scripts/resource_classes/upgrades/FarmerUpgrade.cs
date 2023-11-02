
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
}