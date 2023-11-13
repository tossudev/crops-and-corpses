using Godot;

[GlobalClass, Icon("res://icon.svg")]
public partial class StorageUpgrade : TownUpgrade
{
    [Export] int _storageCapacity;
    public int storageCapacity => _storageCapacity;
    
    public override string GetEffectDescription()
    {
        string description = "";

        if (storageCapacity > 0)
        {
            description = AddDelimiterIfNotEmpty(description);
            description += storageCapacity + " more town storage slots";
        }
        
        return description;
    }
}