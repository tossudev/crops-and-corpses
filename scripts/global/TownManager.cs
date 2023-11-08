using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot.Collections;

public partial class TownManager : Node2D
{
	const string DEFAULT_STATS_PATH = "res://assets/resources/town_stats_upgrades/town_starting_stats.tres";

	public static RawTownStats currentTownStats => SaveData.townHallStats;
	
	public override void _Ready()
	{
	}

	public static async void ReadTownDataFromFile(Dictionary saveData)
	{
		if (await RawTownStats.AssignStatsDataFromDictionary(saveData)) return;
        
		SaveData.townHallStats = ResourceLoader.Load<TownStats>(DEFAULT_STATS_PATH).TownStatsAsRaw();
		await SaveData.SyncTownStats();
	}

	public static void GainExp(TownStats.ExpGain amount)
	{
		SaveData.townHallStats.totalExperience += (int) amount;
	}
    
	
	
	public static void ApplyUpgrade(TownUpgrade upgrade)
	{
		if (upgrade == null)
		{
			GD.PushError("Upgrade was null");
			return;
		}
		
		if (SaveData.appliedUpgrades.Any(townUpgrade => townUpgrade.id == upgrade.id)) return;
		
		switch (upgrade)
		{
			case FarmerUpgrade farmerUpgrade:
				
				SaveData.townHallStats.farmerMaxFarms += farmerUpgrade.farmerMaxFarms;
				SaveData.townHallStats.farmerWalkSpeed += farmerUpgrade.farmerWalkSpeed;
				break;
			
			
			case HousingUpgrade housingUpgrade:
				
				SaveData.townHallStats.houseHP += housingUpgrade.houseHP;
				break;

			
			case SoldierUpgrade soldierUpgrade:

				SaveData.townHallStats.soldierAccuracy += soldierUpgrade.soldierAccuracy;
				SaveData.townHallStats.soldierAttackSpeed += soldierUpgrade.soldierAttackSpeed;
				break;
			
			case StorageUpgrade storageUpgrade:
				
				break;
			case WallUpgrade wallUpgrade:

				SaveData.townHallStats.wallHP = wallUpgrade.wallHP;
				SaveData.townHallStats.spikyWalls = wallUpgrade.spikyWalls;
				break;
		}
		
		SaveData.appliedUpgrades.Add(upgrade);
		Task sync = SaveData.SyncTownStats();
	}
}
