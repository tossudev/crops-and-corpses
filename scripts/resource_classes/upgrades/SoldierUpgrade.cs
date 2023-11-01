using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class SoldierUpgrade : TownUpgrade
{
    [Export] int _soldierAttackSpeed;
    public int soldierAttackSpeed => _soldierAttackSpeed;

    [Export] int _soldierAccuracy;
    public int soldierAccuracy => _soldierAccuracy;
}