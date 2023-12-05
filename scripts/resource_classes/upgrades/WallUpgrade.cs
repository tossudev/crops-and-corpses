using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class WallUpgrade : TownUpgrade
{
    [Export] int _wallHP;
    public int wallHP => _wallHP;
    
    public override string GetEffectDescription()
    {
        string description = "";

        if (wallHP > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            description += "Wall HP +" + wallHP;
        }

        return description;
    }
}