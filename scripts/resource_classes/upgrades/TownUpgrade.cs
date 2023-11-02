using Godot;

public partial class TownUpgrade : Resource
{
	[Export] int _id;
	public int id => _id;

	[Export] CraftingRequirement[] _craftingRequirements;
	public CraftingRequirement[] craftingRequirements => _craftingRequirements;

}