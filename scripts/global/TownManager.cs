using Godot;
using System;

public partial class TownManager : Node2D
{
	[Export] TownStats defaultStats;

	public RawTownStats currentTownStats = new();
	
	public override void _Ready()
	{
		currentTownStats = SaveData.townHallStats;
		if (currentTownStats.totalExperience == 0f)
		{
			currentTownStats = defaultStats.TownStatsAsRaw();
		}
	}

	void GainExp(TownStats.ExpGain amount)
	{
		
	}

	
	void ApplyUpgrade(TownUpgrade upgrade)
	{
		switch (upgrade)
		{
			case null:
				GD.PushError("Upgrade was null");
				break;
			
            
			case FarmerUpgrade farmerUpgrade:
				
				currentTownStats.farmerMaxFarms += farmerUpgrade.farmerMaxFarms;
				currentTownStats.farmerWalkSpeed += farmerUpgrade.farmerWalkSpeed;
				break;
			
			
			case HousingUpgrade housingUpgrade:
				
				currentTownStats.houseHP += housingUpgrade.houseHP;
				break;

			
			case SoldierUpgrade soldierUpgrade:

				currentTownStats.soldierAccuracy += soldierUpgrade.soldierAccuracy;
				currentTownStats.soldierAttackSpeed += soldierUpgrade.soldierAttackSpeed;
				break;
			case StorageUpgrade storageUpgrade:

				break;
			case WallUpgrade wallUpgrade:
				break;
		}
	}
}
