using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class TownUpgrade : Resource
{
	[Export] int _id;
	public int id => _id;
	
	[Export] Texture2D _upgradeIcon;
	public Texture2D upgradeIcon => _upgradeIcon;
	
	[Export] string _upgradeHeader;
	public string upgradeHeader => _upgradeHeader;
	
	[Export] string _upgradeDescription;
	public string upgradeDescription => _upgradeDescription;

	[Export] CraftingRequirement[] _craftingRequirements;
	public CraftingRequirement[] craftingRequirements => _craftingRequirements;

	public TownUpgrade() {}
	public TownUpgrade(int id, string header)
	{
		_id = id;
		_upgradeHeader = header;
	}
	
	public virtual string GetEffectDescription(){return "";}

	protected string AddDelimiterIfNotEmpty(string description)
	{
		const string delimiter = ", ";
		
		return string.IsNullOrEmpty(description)
			? description
			: description + delimiter;
	}

	protected int GetPercentage(float dividend, float divider)
	{
		return (int)(dividend / divider * 100f);
	}

	public bool IsUpgradeApplied ()
	{
		return SaveData.appliedUpgrades.Any(upgrade => upgrade.id == id);
	}


	public static Dictionary GetDictionary(List<TownUpgrade> appliedUpgrades)
	{
		Dictionary appliedUpgradeDict = new ();
        
		foreach (var appliedUpgrade in appliedUpgrades)
		{
			appliedUpgradeDict.Add(appliedUpgrade.id, appliedUpgrade.upgradeHeader);
		}

		return appliedUpgradeDict;
	}
}