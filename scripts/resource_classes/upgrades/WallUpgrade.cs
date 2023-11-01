using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class WallUpgrade : TownUpgrade
{
    [Export] int _wallHP;
    public int wallHP => _wallHP;

    [Export] bool _spikyWalls;
    public bool spikyWalls => _spikyWalls;
}