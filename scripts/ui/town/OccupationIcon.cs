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
	bool _initStarted;
	
	async void Initialize ()
	{
		if (initialized) return;

		_initStarted = true;
		await TaskExtensions.SuspendWhile(() => VillagerManager.villagerManagerInstance == null, 100);
		
		_icon = GetNode<TextureRect>(ICON_NODENAME);
		_icon.Texture = _iconSprite;
		
		_employedAmountLabel = GetNode<Label>(EMPLOYED_LABEL_NODENAME);

		initialized = true;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (!Visible) return;

		if (!initialized)
		{
			if (!_initStarted)
			{
				Initialize();
			}
			return;
		}
        
		
		int employeeAmount = VillagerManager.villagerManagerInstance != null
		? _occupation switch
		{
			VillagerOccupation.Builder => VillagerManager.villagerManagerInstance.BuilderVillagers.Count,
			VillagerOccupation.Farmer => VillagerManager.villagerManagerInstance.farmerVillagers.Count,
			VillagerOccupation.Soldier => VillagerManager.villagerManagerInstance.soldierVillagers.Count,
			VillagerOccupation.Woodcutter => VillagerManager.villagerManagerInstance.woodcutterVillagers.Count,
			VillagerOccupation.Miner => VillagerManager.villagerManagerInstance.minerVillagers.Count,
			_ => -1
		}
		: -1;

		_employedAmountLabel.Text = $"x {employeeAmount}";
	}
}
