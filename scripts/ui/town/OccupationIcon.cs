using Godot;
using System;

public partial class OccupationIcon : Control
{
	TextureRect _icon;
	const string ICON_NODENAME = "%Sprite";
	
	Label _employedAmountLabel;
	const string EMPLOYED_LABEL_NODENAME = "%EmployedAmount";

	[Export] Texture2D _iconSprite;
	[Export] VillagerOccupation _occupation;

	bool initialized;
	
	
	void Initialize ()
	{
		if (initialized) return;
		
		_icon = GetNode<TextureRect>(ICON_NODENAME);
		_icon.Texture = _iconSprite;
		
		_employedAmountLabel = GetNode<Label>(EMPLOYED_LABEL_NODENAME);

		initialized = true;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (!Visible) return;
		
		if (!initialized) Initialize();
        
		int employeeAmount = _occupation switch
		{
			VillagerOccupation.Unemployed => VillagerManager.instance.unemployedVillagers.Count,
			VillagerOccupation.Farmer => VillagerManager.instance.farmerVillagers.Count,
			VillagerOccupation.Soldier => VillagerManager.instance.soldierVillagers.Count,
			VillagerOccupation.Woodcutter => VillagerManager.instance.woodcutterVillagers.Count,
			VillagerOccupation.Miner => VillagerManager.instance.minerVillagers.Count,
			_ => -1
		};

		_employedAmountLabel.Text = $"x {employeeAmount}";
	}
}
