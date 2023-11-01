using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class HousingUpgrade : TownUpgrade
{
    [Export] int _houseHP;
    public int houseHP => _houseHP;
}